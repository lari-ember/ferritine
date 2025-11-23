#!/usr/bin/env python3
"""
Ferritine - Main Entry Point
Sistema de simulação de transporte urbano com integração Unity/C#.
"""

import argparse
import sys
from pathlib import Path

# Adicionar diretório raiz ao path
sys.path.insert(0, str(Path(__file__).parent))

def run_api():
    """Roda API REST para Unity consumir."""
    print("🚀 Iniciando API Ferritine...")
    print("📡 API disponível em: http://localhost:5000")
    print("📚 Documentação em: http://localhost:5000/docs")
    print("\n🔗 Endpoints principais:")
    print("   - GET /api/world/state  (estado completo)")
    print("   - GET /api/stations     (estações)")
    print("   - GET /api/vehicles     (veículos)")
    print("   - GET /api/metrics      (métricas)")
    print("\n💡 Teste com: curl http://localhost:5000/api/world/state")
    print("\nPressione Ctrl+C para parar\n")

    import uvicorn
    uvicorn.run(
        "backend.api.main:app",
        host="0.0.0.0",
        port=5000,
        reload=True,
        log_level="info"
    )

def run_seed():
    """Popula banco com dados iniciais."""
    print("🌱 Populando banco de dados...")
    from scripts.seed_unity_ready import seed_minimal_world
    seed_minimal_world()

def run_demo():
    """Roda demo antiga (backward compatibility)."""
    from time import sleep
    from backend.simulation.models.agente import Agente
    from backend.simulation.models.cidade import Cidade

    print("🎮 Rodando demo antiga...")
    cidade = Cidade()
    cidade.add_agente(Agente("Ana", "CasaA", "Fábrica"))
    cidade.add_agente(Agente("Beto", "CasaB", "Loja"))
    cidade.add_agente(Agente("Clara", "CasaC", "Escola"))

    for hora in range(24):
        cidade.step(hora)
        print(f"{hora:02d}h -> {cidade.snapshot()}")
        sleep(0.1)

def main():
    """Entry point com argumentos."""
    parser = argparse.ArgumentParser(
        description="Ferritine - Simulação de Transporte Urbano",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Exemplos de uso:
  python main.py                  # Roda API (padrão)
  python main.py --seed           # Popula banco de dados
  python main.py --demo           # Roda demo antiga
  python main.py --help           # Mostra esta ajuda
        """
    )

    parser.add_argument(
        "--seed",
        action="store_true",
        help="Popula banco de dados com dados iniciais"
    )

    parser.add_argument(
        "--demo",
        action="store_true",
        help="Roda demo antiga de simulação"
    )

    args = parser.parse_args()

    if args.seed:
        run_seed()
    elif args.demo:
        run_demo()
    else:
        # Padrão: rodar API
        run_api()

if __name__ == "__main__":
    main()
