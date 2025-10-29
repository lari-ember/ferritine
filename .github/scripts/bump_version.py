#!/usr/bin/env python3
# .github/scripts/bump_version.py
"""
Script para atualizar a versão semântica do projeto.

Este script lê o arquivo VERSION, incrementa a versão de acordo com o nível
especificado (patch, minor ou major), atualiza o arquivo e imprime a nova versão.

Uso:
    python bump_version.py --level patch|minor|major

Exemplos:
    python bump_version.py --level patch   # 0.1.0 -> 0.1.1
    python bump_version.py --level minor   # 0.1.0 -> 0.2.0
    python bump_version.py --level major   # 0.1.0 -> 1.0.0
"""
import argparse
from pathlib import Path
import sys

VERSION_FILE = Path("VERSION")


def read_version():
    """
    Lê a versão atual do arquivo VERSION.

    Returns:
        str: Versão atual no formato MAJOR.MINOR.PATCH.
             Retorna "0.0.0" se o arquivo não existir.
    """
    if not VERSION_FILE.exists():
        return "0.0.0"
    return VERSION_FILE.read_text().strip()


def write_version(v):
    """
    Escreve a nova versão no arquivo VERSION.

    Args:
        v (str): Nova versão no formato MAJOR.MINOR.PATCH.
    """
    VERSION_FILE.write_text(v + "\n")


def bump(version, level):
    """
    Incrementa a versão de acordo com o nível especificado.

    Args:
        version (str): Versão atual no formato MAJOR.MINOR.PATCH.
        level (str): Nível de incremento ('patch', 'minor' ou 'major').

    Returns:
        str: Nova versão incrementada.

    Raises:
        ValueError: Se a versão não estiver no formato correto ou se o nível for inválido.
    """
    parts = version.split(".")
    if len(parts) != 3:
        raise ValueError("VERSION must be MAJOR.MINOR.PATCH")
    major, minor, patch = map(int, parts)
    
    if level == "patch":
        patch += 1
    elif level == "minor":
        minor += 1
        patch = 0
    elif level == "major":
        major += 1
        minor = 0
        patch = 0
    else:
        raise ValueError("level must be patch|minor|major")
    
    return f"{major}.{minor}.{patch}"


def main():
    """
    Função principal que processa os argumentos da linha de comando
    e executa o bump da versão.
    """
    p = argparse.ArgumentParser(description="Bump semantic version")
    p.add_argument("--level", choices=["patch", "minor", "major"], default="patch",
                   help="Version level to bump (default: patch)")
    args = p.parse_args()

    cur = read_version()
    try:
        new = bump(cur, args.level)
    except Exception as e:
        print("ERROR:", e, file=sys.stderr)
        sys.exit(1)

    write_version(new)
    # Print the new version as the only stdout line (easy to read in CI)
    print(new)


if __name__ == "__main__":
    main()

