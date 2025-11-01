# agente.py

class Agente:
    """
    Representa um habitante da cidade com uma rotina simples.

    Atributos:
        nome (str): Nome do agente.
        casa (str): Local onde o agente mora.
        trabalho (str): Local onde o agente trabalha.
        local (str): Local atual do agente (inicia na casa).
    """
    def __init__(self, nome: str, casa: str, trabalho: str):
        self.nome = nome
        self.casa = casa
        self.trabalho = trabalho
        self.local = casa  # Estado inicial: o agente começa em casa

    def step(self, hora: int):
        """
        Atualiza o local do agente dependendo da hora do dia.

        Das 7h às 17h, o agente está no trabalho.
        Fora desse intervalo, o agente está em casa.

        Args:
            hora (int): Hora atual (0-23).
        """
        if 7 <= hora < 17:
            self.local = self.trabalho
        else:
            self.local = self.casa

    def __repr__(self):
        """
        Retorna uma representação legível do agente.

        Returns:
            str: Representação no formato "<Agente Nome @ Local>".
        """
        return f"<Agente {self.nome} @ {self.local}>"
