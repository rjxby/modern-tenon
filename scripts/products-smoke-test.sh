#!/usr/bin/env bash

set -euo pipefail

BASE_URL="${BASE_URL:-http://127.0.0.1:8080}"
TMP_DIR="$(mktemp -d)"
GREEN="$(printf '\033[32m')"
RED="$(printf '\033[31m')"
BLUE="$(printf '\033[34m')"
YELLOW="$(printf '\033[33m')"
BOLD="$(printf '\033[1m')"
RESET="$(printf '\033[0m')"

cleanup() {
  rm -rf "${TMP_DIR}"
}

trap cleanup EXIT

require_command() {
  if ! command -v "$1" >/dev/null 2>&1; then
    echo "Missing required command: $1" >&2
    exit 1
  fi
}

log_step() {
  printf '%b\n' "${BLUE}${BOLD}==>${RESET} $1"
}

log_info() {
  printf '%b\n' "${YELLOW}INFO${RESET} $1"
}

log_pass() {
  printf '%b\n' "${GREEN}PASS${RESET} $1"
}

log_fail() {
  printf '%b\n' "${RED}FAIL${RESET} $1" >&2
}

print_response() {
  local file_path="$1"
  printf '%b\n' "${BLUE}Response:${RESET} $(tr '\n' ' ' < "${file_path}")"
}

wait_for_api() {
  local attempts=30
  local last_error=""

  log_step "Waiting for API at ${BASE_URL}"

  for ((i = 1; i <= attempts; i++)); do
    last_error="$(curl --silent --show-error --fail "${BASE_URL}/api/products?page=1&limit=1" >/dev/null 2>&1 || true)"
    if [[ -z "${last_error}" ]]; then
      log_pass "API is ready"
      return 0
    fi

    if (( i == 1 )); then
      log_info "Polling ${BASE_URL}/api/products?page=1&limit=1 for up to ${attempts} seconds"
    fi

    sleep 1
  done

  log_fail "API did not become ready at ${BASE_URL}"
  if [[ -n "${last_error}" ]]; then
    printf '%b\n' "${RED}FAIL${RESET} Last curl error: ${last_error}" >&2
  fi
  exit 1
}

assert_status() {
  local actual="$1"
  local expected="$2"
  local description="$3"

  if [[ "${actual}" != "${expected}" ]]; then
    log_fail "${description}: expected HTTP ${expected}, got ${actual}"
    exit 1
  fi

  log_pass "${description}: HTTP ${actual}"
}

assert_body_contains() {
  local file_path="$1"
  local expected="$2"
  local description="$3"

  if ! grep -q "${expected}" "${file_path}"; then
    log_fail "${description}: response did not contain ${expected}"
    cat "${file_path}" >&2
    exit 1
  fi

  log_pass "${description}: response contained ${expected}"
}

require_command curl
require_command grep
require_command sed

wait_for_api

LIST_FILE="${TMP_DIR}/list.json"
CREATE_FILE="${TMP_DIR}/create.json"
GET_FILE="${TMP_DIR}/get.json"
UPDATE_FILE="${TMP_DIR}/update.json"

CREATE_NAME="Smoke Test Product $(date +%s)"
UPDATE_NAME="Updated Smoke Test Product $(date +%s)"

log_step "GET ${BASE_URL}/api/products?page=1&limit=10"
list_status="$(curl --silent --show-error --output "${LIST_FILE}" --write-out "%{http_code}" "${BASE_URL}/api/products?page=1&limit=10")"
assert_status "${list_status}" "200" "List products"
assert_body_contains "${LIST_FILE}" '"results"' "List products"
print_response "${LIST_FILE}"

log_step "POST ${BASE_URL}/api/products/"
log_info "Request body: {\"name\":\"${CREATE_NAME}\"}"
create_status="$(curl --silent --show-error --output "${CREATE_FILE}" --write-out "%{http_code}" \
  -X POST "${BASE_URL}/api/products/" \
  -H "Content-Type: application/json" \
  -d "{\"name\":\"${CREATE_NAME}\"}")"
assert_status "${create_status}" "200" "Create product"
assert_body_contains "${CREATE_FILE}" "\"name\":\"${CREATE_NAME}\"" "Create product"
print_response "${CREATE_FILE}"

product_id="$(sed -n 's/.*"id":"\([^"]*\)".*/\1/p' "${CREATE_FILE}")"
if [[ -z "${product_id}" ]]; then
  log_fail "Create product: could not extract product id"
  cat "${CREATE_FILE}" >&2
  exit 1
fi

log_info "Created product id: ${product_id}"

log_step "GET ${BASE_URL}/api/products/${product_id}"
get_status="$(curl --silent --show-error --output "${GET_FILE}" --write-out "%{http_code}" \
  "${BASE_URL}/api/products/${product_id}")"
assert_status "${get_status}" "200" "Get created product"
assert_body_contains "${GET_FILE}" "\"id\":\"${product_id}\"" "Get created product"
print_response "${GET_FILE}"

log_step "PUT ${BASE_URL}/api/products/${product_id}"
log_info "Request body: {\"name\":\"${UPDATE_NAME}\",\"price\":149.99}"
update_status="$(curl --silent --show-error --output "${UPDATE_FILE}" --write-out "%{http_code}" \
  -X PUT "${BASE_URL}/api/products/${product_id}" \
  -H "Content-Type: application/json" \
  -d "{\"name\":\"${UPDATE_NAME}\",\"price\":149.99}")"
assert_status "${update_status}" "200" "Update product"
assert_body_contains "${UPDATE_FILE}" "\"name\":\"${UPDATE_NAME}\"" "Update product"
assert_body_contains "${UPDATE_FILE}" '"price":149.99' "Update product"
print_response "${UPDATE_FILE}"

printf '%b\n' "${GREEN}${BOLD}Smoke test passed against ${BASE_URL}${RESET}"
