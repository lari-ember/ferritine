# main.py
from time import sleep
from app.models.agente import Agente
from app.models.cidade import Cidade

# Função principal que executa a demonstração da simulação
# Cria uma cidade, adiciona agentes e simula suas rotinas ao longo de 24 horas
def run_demo():
    # Criação de uma instância da cidade
    cidade = Cidade()

    # Adicionando agentes à cidade com suas respectivas casas e locais de trabalho
    cidade.add_agente(Agente("Ana", "CasaA", "Fábrica"))
    cidade.add_agente(Agente("Beto", "CasaB", "Loja"))
    cidade.add_agente(Agente("Clara", "CasaC", "Escola"))

    # Simulação de 24 horas, avançando uma hora por iteração
    for hora in range(24):
        cidade.step(hora)  # Atualiza o estado da cidade para a hora atual
        print(f"{hora:02d}h -> {cidade.snapshot()}")  # Mostra o estado atual da cidade
        sleep(0.1)  # Pausa para tornar a saída mais lenta e legível

# Ponto de entrada do programa
if __name__ == "__main__":
    run_demo()
