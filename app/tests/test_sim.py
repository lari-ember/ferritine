"""
Testes unitários para o sistema de simulação Ferritine.

Este módulo contém testes para validar o comportamento dos agentes e da cidade
na simulação. Os testes garantem que:
- Os agentes seguem suas rotinas diárias corretamente
- A cidade gerencia os agentes adequadamente
- Os snapshots refletem o estado correto do sistema

Autor: Equipe Ferritine
Data: 2025-10-29
"""

from app.models.agente import Agente
from app.models.cidade import Cidade


def test_agente_move_para_trabalho():
    """
    Testa se o agente se move corretamente entre casa e trabalho.

    Este teste verifica se o agente:
    1. Está no trabalho durante o horário de expediente (10h)
    2. Retorna para casa fora do expediente (20h)

    Horário de trabalho considerado: 7h às 17h
    """
    # Arrange: Cria um agente com casa e trabalho definidos
    agente = Agente("TestAgent", "Casa", "Trabalho")

    # Act & Assert: Verifica movimento para o trabalho às 10h
    agente.step(10)
    assert agente.local == "Trabalho", "Agente deveria estar no trabalho às 10h"

    # Act & Assert: Verifica retorno para casa às 20h
    agente.step(20)
    assert agente.local == "Casa", "Agente deveria estar em casa às 20h"


def test_cidade_snapshot():
    """
    Testa se o snapshot da cidade inclui os agentes adicionados.

    Este teste verifica se:
    1. Os agentes são corretamente adicionados à cidade
    2. O snapshot contém informações dos agentes
    3. O estado dos agentes é atualizado após step()
    """
    # Arrange: Cria uma cidade e adiciona um agente
    cidade = Cidade()
    agente = Agente("AgentX", "CasaX", "TrabalhoX")
    cidade.add_agente(agente)

    # Act: Atualiza o estado da cidade para o horário 9h (horário de trabalho)
    cidade.step(9)
    snapshot = cidade.snapshot()

    # Assert: Verifica se o agente está presente no snapshot
    assert "AgentX" in snapshot, "Agente deveria estar presente no snapshot"
    assert snapshot["AgentX"] == "TrabalhoX", "Agente deveria estar no trabalho às 9h"


def test_agente_horarios_limites():
    """
    Testa os horários limites de transição casa-trabalho.

    Verifica se o agente:
    - Está em casa às 6h (antes do expediente)
    - Está no trabalho às 7h (início do expediente)
    - Está no trabalho às 16h (ainda no expediente)
    - Está em casa às 17h (fim do expediente)
    """
    agente = Agente("LimiteAgent", "MinhaCasa", "MeuTrabalho")

    # Antes do expediente (6h)
    agente.step(6)
    assert agente.local == "MinhaCasa", "Agente deveria estar em casa às 6h"

    # Início do expediente (7h)
    agente.step(7)
    assert agente.local == "MeuTrabalho", "Agente deveria estar no trabalho às 7h"

    # Durante o expediente (16h)
    agente.step(16)
    assert agente.local == "MeuTrabalho", "Agente deveria estar no trabalho às 16h"

    # Fim do expediente (17h)
    agente.step(17)
    assert agente.local == "MinhaCasa", "Agente deveria estar em casa às 17h"


def test_cidade_multiplos_agentes():
    """
    Testa o gerenciamento de múltiplos agentes na cidade.

    Verifica se a cidade consegue:
    - Adicionar múltiplos agentes
    - Atualizar todos os agentes simultaneamente
    - Gerar snapshots com todos os agentes
    """
    cidade = Cidade()

    # Adiciona múltiplos agentes
    cidade.add_agente(Agente("Ana", "CasaA", "EscolaA"))
    cidade.add_agente(Agente("Beto", "CasaB", "EscolaB"))
    cidade.add_agente(Agente("Carlos", "CasaC", "EscolaC"))

    # Atualiza para horário de trabalho (10h)
    cidade.step(10)
    snapshot = cidade.snapshot()

    # Verifica se todos os agentes estão no trabalho
    assert len(snapshot) == 3, "Snapshot deveria conter 3 agentes"
    assert snapshot["Ana"] == "EscolaA", "Ana deveria estar na EscolaA"
    assert snapshot["Beto"] == "EscolaB", "Beto deveria estar na EscolaB"
    assert snapshot["Carlos"] == "EscolaC", "Carlos deveria estar na EscolaC"

    # Atualiza para fora do horário de trabalho (20h)
    cidade.step(20)
    snapshot = cidade.snapshot()

    # Verifica se todos os agentes estão em casa
    assert snapshot["Ana"] == "CasaA", "Ana deveria estar na CasaA"
    assert snapshot["Beto"] == "CasaB", "Beto deveria estar na CasaB"
    assert snapshot["Carlos"] == "CasaC", "Carlos deveria estar na CasaC"
