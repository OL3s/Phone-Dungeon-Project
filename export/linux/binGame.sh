#!/bin/sh
echo -ne '\033c\033]0;Path of Exhile\a'
base_path="$(dirname "$(realpath "$0")")"
"$base_path/binGame.x86_64" "$@"
