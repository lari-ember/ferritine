# test_sim.py
from app.models.agente import Agente
from app.models.cidade import Cidade

# Testa se o agente se move corretamente entre casa e trabalho
# Verifica se o agente está no trabalho durante o horário de expediente (10h)
# e retorna para casa fora do expediente (20h)
def test_agente_move_para_trabalho():
    a = Agente("T", "Casa", "Trab")  # Cria um agente com casa e trabalho definidos
    a.step(10)  # Atualiza o estado do agente para o horário 10h
    assert a.local == "Trab"  # Verifica se o agente está no trabalho
    a.step(20)  # Atualiza o estado do agente para o horário 20h
    assert a.local == "Casa"  # Verifica se o agente está em casa

# Testa se o snapshot da cidade inclui os agentes adicionados
# Verifica se o agente "X" está presente no snapshot após um passo de simulação
def test_cidade_snapshot():
    c = Cidade()  # Cria uma nova cidade
    c.add_agente(Agente("X","C","T"))  # Adiciona um agente à cidade
    c.step(9)  # Atualiza o estado da cidade para o horário 9h
    snap = c.snapshot()  # Obtém o snapshot da cidade
    assert "X" in snap  # Verifica se o agente "X" está no snapshot
