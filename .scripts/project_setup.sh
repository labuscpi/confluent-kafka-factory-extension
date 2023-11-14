#!/bin/bash

print_header() {

  # Reset
  local Color_Off='\033[0m'       # Text Reset

  # Regular Colors
  local Black='\033[0;30m'        # Black
  local Red='\033[0;31m'          # Red
  local Green='\033[0;32m'        # Green
  local Yellow='\033[0;33m'       # Yellow
  local Blue='\033[0;34m'         # Blue
  local Purple='\033[0;35m'       # Purple
  local Cyan='\033[0;36m'         # Cyan
  local White='\033[0;37m'        # White
  local COLOR=$Green

  if [ -z "${2}" ]; then
    COLOR=$Green
  else
    [[ "${2}" == 'Black' ]] && COLOR=${Black}
    [[ "${2}" == 'Red' ]] && COLOR=${Red}
    [[ "${2}" == 'Green' ]] && COLOR=${Green}
    [[ "${2}" == 'Yellow' ]] && COLOR=${Yellow}
    [[ "${2}" == 'Blue' ]] && COLOR=${Blue}
    [[ "${2}" == 'Purple' ]] && COLOR=${Purple}
    [[ "${2}" == 'Cyan' ]] && COLOR=${Cyan}
    [[ "${2}" == 'White' ]] && COLOR=${White}
  fi

  echo -e "${COLOR}==> ${White}${1}${Color_Off}"
}

check_package() {
  echo "$(which "${1}" 2>&1 >/dev/null; echo $?)"
}

project_config() {
  if [ "$(check_package 'gitversion')" != 0 ]; then
    print_header "Running 'brew install gitversion'" 'Purple'
    brew install gitversion
  fi

  if [ "$(check_package 'pre-commit')" != 0 ]; then
    print_header "Running 'brew install pre-commit'" 'Purple'
    brew install pre-commit
  fi

  if [ ! -f '.git/hooks/pre-commit' ]; then
    print_header "Running 'pre-commit install; pre-commit autoupdate'" 'Purple'
    pre-commit install; pre-commit autoupdate
  fi

  if [ ! -f '.git/hooks/commit-msg' ]; then
    print_header "Running 'pre-commit install --hook-type commit-msg'" 'Purple'
    pre-commit install --hook-type commit-msg
  fi

  if [ "$(check_package 'cz')" != 0 ]; then
    print_header "Running 'brew install commitizen'" 'Purple'
    brew install commitizen
  fi

  print_header "Running 'pre-commit run --all-files'" 'Purple'
  pre-commit run --all-files
}

print_header 'Check Project Setup'
if [ "$(check_package 'brew')" == 0 ]; then
  project_config

else
  print_header 'Action required: install brew package management' 'Red'
  echo '# /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"'
  echo 'after successful installation of brew execute:'
  echo '1) install pre-commit: '
  echo '     brew install pre-commit'
  echo '2) install pre-commit hooks: '
  echo '     2.1) pre-commit install'
  echo '     2.2) pre-commit autoupdate (Only if asked)'
  echo '     2.3) pre-commit install --hook-type commit-msg'
  echo '     2.4) pre-commit run --all-files (Optional)'

fi
