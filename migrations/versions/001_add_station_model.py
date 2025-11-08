"""add_station_model

Revision ID: 001
Revises:
Create Date: 2025-11-08 21:43:31.000000

"""

from typing import Sequence, Union

from alembic import op
import sqlalchemy as sa
from sqlalchemy.dialects import postgresql

# revision identifiers, used by Alembic.
revision: str = "001"
down_revision: Union[str, None] = None
branch_labels: Union[str, Sequence[str], None] = None
depends_on: Union[str, Sequence[str], None] = None


def upgrade() -> None:
    """Create stations table."""
    # Create StationType enum
    station_type_enum = postgresql.ENUM(
        "train_station",
        "bus_stop",
        "bus_terminal",
        "tram_stop",
        "taxi_stand",
        "truck_depot",
        "metro_station",
        "airport",
        "port",
        name="stationtype",
        create_type=False,
    )
    station_type_enum.create(op.get_bind(), checkfirst=True)

    # Create stations table
    op.create_table(
        "stations",
        sa.Column("id", sa.CHAR(36), nullable=False),
        sa.Column("building_id", sa.CHAR(36), nullable=True),
        sa.Column("name", sa.String(200), nullable=False),
        sa.Column("type", station_type_enum, nullable=False),
        sa.Column("platform_count", sa.Integer(), nullable=True, server_default="1"),
        sa.Column(
            "max_vehicles_docked", sa.Integer(), nullable=True, server_default="2"
        ),
        sa.Column("has_shelter", sa.Boolean(), nullable=True, server_default="false"),
        sa.Column(
            "has_ticket_office", sa.Boolean(), nullable=True, server_default="false"
        ),
        sa.Column("has_restrooms", sa.Boolean(), nullable=True, server_default="false"),
        sa.Column(
            "condition_percent", sa.Integer(), nullable=True, server_default="100"
        ),
        sa.Column("last_inspection_date", sa.DateTime(), nullable=True),
        sa.Column("created_at", sa.DateTime(), nullable=False),
        sa.Column("updated_at", sa.DateTime(), nullable=True),
        sa.PrimaryKeyConstraint("id"),
        sa.ForeignKeyConstraint(["building_id"], ["buildings.id"]),
        sa.CheckConstraint(
            "condition_percent >= 0 AND condition_percent <= 100",
            name="check_station_condition_range",
        ),
    )

    # Create indexes
    op.create_index("ix_stations_name", "stations", ["name"])


def downgrade() -> None:
    """Drop stations table."""
    op.drop_index("ix_stations_name", table_name="stations")
    op.drop_table("stations")

    # Drop StationType enum
    station_type_enum = postgresql.ENUM(
        "train_station",
        "bus_stop",
        "bus_terminal",
        "tram_stop",
        "taxi_stand",
        "truck_depot",
        "metro_station",
        "airport",
        "port",
        name="stationtype",
    )
    station_type_enum.drop(op.get_bind(), checkfirst=True)
