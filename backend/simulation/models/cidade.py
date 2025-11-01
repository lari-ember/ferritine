# cidade.py
from typing import List
from .agente import Agente
from backend.utils.logger import get_logger

logger = get_logger(__name__)


class Cidade:
    """
    Contém e gerencia agentes; é o "mundo" da simulação.

    Atributos:
        agentes (List[Agente]): Lista de agentes presentes na cidade.
    """
    def __init__(self, agentes: List[Agente] = None):
        self.agentes = agentes or []  # Inicializa com uma lista vazia se nenhum agente for fornecido
        logger.info("Cidade criada com %d agentes", len(self.agentes))

    def add_agente(self, agente: Agente):
        """
        Adiciona um agente à cidade.

        Args:
            agente (Agente): O agente a ser adicionado.
        """
        self.agentes.append(agente)
        logger.debug("Agente %s adicionado à cidade", agente.nome)

    def step(self, hora: int):
        """
        Avança a simulação uma unidade de tempo (hora).

        Atualiza o estado de todos os agentes na cidade com base na hora atual.

        Args:
            hora (int): Hora atual (0-23).
        """
        logger.debug("Processando step da cidade para hora %d com %d agentes", hora, len(self.agentes))
        for agente in self.agentes:
            agente.step(hora)

    def snapshot(self):
        """
        Retorna um resumo legível do estado atual da cidade.

        Returns:
            dict: Um dicionário com o nome dos agentes como chaves e seus locais como valores.
        """
        return {a.nome: a.local for a in self.agentes}
