# bump_version.py
from pathlib import Path

def bump_patch(path="VERSION"):
    p = Path(path)
    v = p.read_text().strip()
    major, minor, patch = map(int, v.split("."))
    patch += 1
    new = f"{major}.{minor}.{patch}"
    p.write_text(new)
    print(new)

if __name__ == "__main__":
    bump_patch()
