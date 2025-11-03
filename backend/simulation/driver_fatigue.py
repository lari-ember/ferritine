"""
Sistema de fadiga de motoristas/maquinistas.
Simula cansaço da tripulação e impactos na segurança.
"""


class DriverFatigueSystem:
    """
    Sistema que simula fadiga de motoristas/maquinistas
    """

    def __init__(self, db):
        self.db = db
        self.fatigue_levels = {}  # {driver_id: fatigue_percent}

    def update_fatigue(self, driver_id: int, hours_worked: float):
        """
        Atualiza nível de fadiga
        """
        if driver_id not in self.fatigue_levels:
            self.fatigue_levels[driver_id] = 0

        # Fadiga aumenta com horas trabalhadas
        fatigue_increase = hours_worked * 5  # 5% por hora
        self.fatigue_levels[driver_id] += fatigue_increase

        # Máximo 100%
        self.fatigue_levels[driver_id] = min(100, self.fatigue_levels[driver_id])

    def reset_fatigue(self, driver_id: int):
        """
        Reseta fadiga após descanso
        """
        self.fatigue_levels[driver_id] = 0

    def get_fatigue_level(self, driver_id: int) -> float:
        """
        Retorna nível de fadiga atual
        """
        return self.fatigue_levels.get(driver_id, 0)

    def is_too_tired(self, driver_id: int, threshold: float = 80.0) -> bool:
        """
        Verifica se motorista está muito cansado
        """
        return self.get_fatigue_level(driver_id) >= threshold

    def calculate_accident_risk(self, driver_id: int) -> float:
        """
        Calcula risco de acidente baseado na fadiga

        Returns:
            float: Risco de 0.0 (sem risco) a 1.0 (altíssimo risco)
        """
        fatigue = self.get_fatigue_level(driver_id)

        # Risco aumenta exponencialmente com fadiga
        if fatigue < 50:
            return fatigue / 200  # 0-25% de risco
        elif fatigue < 80:
            return 0.25 + (fatigue - 50) / 100  # 25-55% de risco
        else:
            return 0.55 + (fatigue - 80) / 40  # 55-100% de risco
