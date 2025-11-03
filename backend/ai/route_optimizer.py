"""
IA para otimização de rotas e frequência de veículos.
Analisa demanda histórica e sugere ajustes.
"""

from typing import Dict


class RouteOptimizer:
    """
    IA que otimiza frequência de veículos baseado em demanda
    """

    def __init__(self, db):
        self.db = db

    def analyze_demand(self, route_id: int, time_window_days: int = 30) -> Dict:
        """
        Analisa demanda histórica de uma rota
        """
        cursor = self.db.conn.cursor()
        cursor.execute(
            """
            SELECT
                strftime('%H', t.started_at) as hour,
                AVG(t.passengers_boarded) as avg_passengers,
                AVG(CAST(t.passengers_boarded AS REAL) / v.max_passengers * 100) as avg_occupancy
            FROM trips t
            JOIN vehicles v ON t.vehicle_id = v.id
            WHERE t.route_id = ?
            AND t.started_at >= date('now', '-{} days')
            GROUP BY hour
            ORDER BY hour
        """.format(
                time_window_days
            ),
            (route_id,),
        )

        data = cursor.fetchall()

        return {
            "hourly_demand": [(row[0], row[1], row[2]) for row in data],
            "peak_hours": [row[0] for row in data if row[2] > 80],  # Ocupação > 80%
            "low_hours": [row[0] for row in data if row[2] < 30],  # Ocupação < 30%
        }

    def suggest_frequency_adjustment(self, route_id: int) -> Dict:
        """
        Sugere ajustes na frequência de veículos
        """
        analysis = self.analyze_demand(route_id)

        suggestions = {
            "increase_frequency": [],  # Horários que precisam mais veículos
            "decrease_frequency": [],  # Horários que podem reduzir
            "add_express": [],  # Horários para linhas expressas
        }

        for hour, avg_pass, occupancy in analysis["hourly_demand"]:
            if occupancy > 85:
                suggestions["increase_frequency"].append(
                    {"hour": hour, "current_occupancy": occupancy, "recommendation": "Adicionar veículo extra"}
                )
            elif occupancy < 25:
                suggestions["decrease_frequency"].append(
                    {"hour": hour, "current_occupancy": occupancy, "recommendation": "Reduzir frequência em 50%"}
                )
            elif occupancy > 70 and avg_pass > 150:
                suggestions["add_express"].append(
                    {
                        "hour": hour,
                        "current_occupancy": occupancy,
                        "recommendation": "Criar linha expressa (menos paradas)",
                    }
                )

        return suggestions
