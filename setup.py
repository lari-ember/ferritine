"""
Setup script para o projeto Ferritine.

Este arquivo permite instalar o pacote em modo de desenvolvimento
para facilitar imports e testes.

Uso:
    pip install -e .
"""
from setuptools import setup, find_packages
from pathlib import Path

# Lê a versão do arquivo VERSION
VERSION_FILE = Path(__file__).parent / "VERSION"
version = VERSION_FILE.read_text().strip() if VERSION_FILE.exists() else "0.0.0"

# Lê o README para a descrição longa
README_FILE = Path(__file__).parent / "README.md"
long_description = README_FILE.read_text(encoding="utf-8") if README_FILE.exists() else ""

setup(
    name="ferritine",
    version=version,
    author="Equipe Ferritine",
    description="Simulação de maquete híbrida físico-digital com agentes inteligentes",
    long_description=long_description,
    long_description_content_type="text/markdown",
    url="https://github.com/ferritine/ferritine",
    packages=find_packages(exclude=["tests", "*.tests", "*.tests.*", "tests.*"]),
    classifiers=[
        "Development Status :: 3 - Alpha",
        "Intended Audience :: Education",
        "Intended Audience :: Science/Research",
        "License :: OSI Approved :: MIT License",
        "Programming Language :: Python :: 3",
        "Programming Language :: Python :: 3.8",
        "Programming Language :: Python :: 3.9",
        "Programming Language :: Python :: 3.10",
        "Programming Language :: Python :: 3.11",
        "Topic :: Scientific/Engineering :: Artificial Intelligence",
        "Topic :: System :: Distributed Computing",
    ],
    python_requires=">=3.8",
    install_requires=[
        # Dependências principais (mínimas por enquanto)
    ],
    extras_require={
        "dev": [
            "pytest>=8.4.2",
            "pytest-cov>=4.0.0",
            "black>=23.0.0",
            "flake8>=6.0.0",
            "mypy>=1.0.0",
        ],
    },
    entry_points={
        "console_scripts": [
            "ferritine=main:run_demo",
        ],
    },
)

