#!/usr/bin/env python3
"""
Convert CWB Heightmap to Voxel Chunks
=====================================

Script de conveniência para converter o heightmap cwb.png em chunks de voxels.
Pode exportar para arquivo .fvox ou diretamente para o banco de dados.

Uso:
    python scripts/convert_cwb_heightmap.py [--output FILE] [--database]

Opções:
    --output, -o    Arquivo de saída .fvox (default: data/voxel/cwb.fvox)
    --database, -d  Salvar diretamente no PostgreSQL
    --max-height    Altura máxima em metros (default: 200)
    --water-level   Nível da água normalizado 0-1 (default: 0.08)
    --sample        Gerar apenas área de sample (10x10 chunks) para teste

Exemplo:
    python scripts/convert_cwb_heightmap.py -o data/voxel/cwb.fvox
    python scripts/convert_cwb_heightmap.py --database --sample
"""

import argparse
import sys
import os
import logging
from pathlib import Path
from datetime import datetime

# Add project root to path
sys.path.insert(0, str(Path(__file__).parent.parent))

from backend.voxel.heightmap_converter import (
    HeightmapConverter, 
    HeightmapConfig,
    convert_heightmap,
    CHUNK_SIZE,
    VOXEL_SIZE_M
)
from backend.voxel.chunk_manager import ChunkManager

# Configure logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)

# Default paths
PROJECT_ROOT = Path(__file__).parent.parent
HEIGHTMAP_PATH = PROJECT_ROOT / "ferritineVU" / "Assets" / "Sprites" / "nap" / "cwb.png"
DEFAULT_OUTPUT = PROJECT_ROOT / "data" / "voxel" / "cwb.fvox"


def progress_callback(current: int, total: int):
    """Print progress to console."""
    percent = (current / total) * 100
    bar_length = 50
    filled = int(bar_length * current / total)
    bar = '█' * filled + '░' * (bar_length - filled)
    print(f'\r[{bar}] {percent:.1f}% ({current}/{total})', end='', flush=True)


def convert_to_file(
    heightmap_path: str,
    output_path: str,
    config: HeightmapConfig,
    sample_only: bool = False
) -> dict:
    """
    Convert heightmap to .fvox file.
    
    Args:
        heightmap_path: Path to PNG heightmap
        output_path: Path for output .fvox file
        config: Conversion configuration
        sample_only: If True, only generate 10x10 chunk area
        
    Returns:
        Conversion statistics
    """
    logger.info(f"Converting: {heightmap_path}")
    logger.info(f"Output: {output_path}")
    logger.info(f"Config: max_height={config.max_altitude_m}m, water_level={config.water_level}")
    
    if sample_only:
        # Reduce real dimensions for sample
        config.real_width_m = 10 * CHUNK_SIZE * VOXEL_SIZE_M * 10  # ~23m
        config.real_height_m = 10 * CHUNK_SIZE * VOXEL_SIZE_M * 10
        logger.info("SAMPLE MODE: Generating only 10x10 chunk area")
    
    # Create output directory
    Path(output_path).parent.mkdir(parents=True, exist_ok=True)
    
    # Convert
    start_time = datetime.now()
    stats = convert_heightmap(
        heightmap_path,
        output_path,
        config,
        progress_callback=progress_callback
    )
    duration = datetime.now() - start_time
    
    print()  # New line after progress bar
    
    logger.info(f"Conversion complete in {duration}")
    logger.info(f"Chunks generated: {stats['chunk_count']}")
    logger.info(f"File size: {stats['file_size_bytes'] / 1024 / 1024:.2f} MB")
    
    return stats


def convert_to_database(
    heightmap_path: str,
    config: HeightmapConfig,
    sample_only: bool = False
) -> dict:
    """
    Convert heightmap directly to PostgreSQL database.
    
    Args:
        heightmap_path: Path to PNG heightmap
        config: Conversion configuration
        sample_only: If True, only generate 10x10 chunk area
        
    Returns:
        Conversion statistics
    """
    from backend.database.connection import get_session
    
    logger.info(f"Converting to database: {heightmap_path}")
    logger.info(f"Config: max_height={config.max_altitude_m}m, water_level={config.water_level}")
    
    if sample_only:
        config.real_width_m = 10 * CHUNK_SIZE * VOXEL_SIZE_M * 10
        config.real_height_m = 10 * CHUNK_SIZE * VOXEL_SIZE_M * 10
        logger.info("SAMPLE MODE: Generating only 10x10 chunk area")
    
    # Initialize
    converter = HeightmapConverter(heightmap_path, config)
    
    with get_session() as session:
        manager = ChunkManager(session)
        
        start_time = datetime.now()
        chunks_saved = 0
        batch = []
        batch_size = 100
        
        for chunk in converter.generate_chunks(progress_callback):
            batch.append(chunk)
            
            if len(batch) >= batch_size:
                saved = manager.save_chunks_batch(batch, batch_size=batch_size)
                chunks_saved += saved
                batch = []
        
        # Save remaining
        if batch:
            saved = manager.save_chunks_batch(batch, batch_size=len(batch))
            chunks_saved += saved
        
        duration = datetime.now() - start_time
    
    print()  # New line after progress bar
    
    stats = converter.stats
    stats['chunks_saved'] = chunks_saved
    stats['duration_seconds'] = duration.total_seconds()
    
    logger.info(f"Conversion complete in {duration}")
    logger.info(f"Chunks generated: {stats['chunks_generated']}")
    logger.info(f"Chunks saved to DB: {chunks_saved}")
    
    return stats


def main():
    parser = argparse.ArgumentParser(
        description='Convert CWB heightmap to voxel chunks',
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog=__doc__
    )
    
    parser.add_argument(
        '-o', '--output',
        type=str,
        default=str(DEFAULT_OUTPUT),
        help=f'Output .fvox file (default: {DEFAULT_OUTPUT})'
    )
    
    parser.add_argument(
        '-d', '--database',
        action='store_true',
        help='Save directly to PostgreSQL instead of file'
    )
    
    parser.add_argument(
        '-i', '--input',
        type=str,
        default=str(HEIGHTMAP_PATH),
        help=f'Input heightmap PNG (default: {HEIGHTMAP_PATH})'
    )
    
    parser.add_argument(
        '--max-height',
        type=float,
        default=200.0,
        help='Maximum terrain height in meters (default: 200)'
    )
    
    parser.add_argument(
        '--water-level',
        type=float,
        default=0.08,
        help='Water level normalized 0-1 (default: 0.08)'
    )
    
    parser.add_argument(
        '--sample',
        action='store_true',
        help='Generate only 10x10 chunk sample area for testing'
    )
    
    parser.add_argument(
        '-v', '--verbose',
        action='store_true',
        help='Enable verbose logging'
    )
    
    args = parser.parse_args()
    
    if args.verbose:
        logging.getLogger().setLevel(logging.DEBUG)
    
    # Validate input
    if not Path(args.input).exists():
        logger.error(f"Heightmap not found: {args.input}")
        sys.exit(1)
    
    # Create config
    config = HeightmapConfig(
        max_altitude_m=args.max_height,
        water_level=args.water_level
    )
    
    # Convert
    try:
        if args.database:
            stats = convert_to_database(args.input, config, args.sample)
        else:
            stats = convert_to_file(args.input, args.output, config, args.sample)
        
        # Print summary
        print("\n" + "=" * 60)
        print("CONVERSION SUMMARY")
        print("=" * 60)
        for key, value in stats.items():
            if isinstance(value, float):
                print(f"  {key}: {value:.2f}")
            else:
                print(f"  {key}: {value}")
        print("=" * 60)
        
    except KeyboardInterrupt:
        logger.info("\nConversion cancelled by user")
        sys.exit(1)
    except Exception as e:
        logger.error(f"Conversion failed: {e}")
        if args.verbose:
            import traceback
            traceback.print_exc()
        sys.exit(1)


if __name__ == "__main__":
    main()

