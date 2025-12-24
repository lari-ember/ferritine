# ⚡ START HERE - Backend para Unity em 3 Comandos

```bash
# 1️⃣ Instalar dependências
pip install fastapi uvicorn[standard] pydantic

# 2️⃣ Popular banco
python main.py --seed

# 3️⃣ Rodar API
python main.py
```

**✅ Pronto! API rodando em http://localhost:5000**

---

## 🧪 Testar

```bash
# No navegador:
http://localhost:5000/docs

# Ou terminal:
curl http://localhost:5000/api/world/state
```

---

## 🎮 Próximo Passo: Unity

👉 **Abra**: `docs/UNITY_INTEGRATION_GUIDE.md`

Esse guia tem **TODO** o código C# pronto. É só copiar e colar!

---

## 📁 Arquivos Importantes

| Arquivo | O Que É |
|---------|---------|
| `QUICKSTART_UNITY.md` | Quick start de 5 min ⭐ |
| `docs/UNITY_INTEGRATION_GUIDE.md` | Guia completo Unity 📚 |
| `INSTALL_DEPENDENCIES.md` | Se der problema de instalação 🔧 |
| `IMPLEMENTATION_SUMMARY.md` | Resumo completo do que foi feito 📊 |

---

## 🐛 Deu Erro?

**"No module named 'fastapi'"**
```bash
pip install fastapi uvicorn[standard]
```

**"Connection refused"**
```bash
# API está rodando?
python main.py
```

**"No such file or directory"**
```bash
# Verifique que está na pasta correta
cd ~/Documentos/codigos/ferritine
python main.py --seed
```

---

## 🎯 Endpoints Principais

- `GET /` → Status
- `GET /api/world/state` → ⭐ **Estado completo (Unity usa este)**
- `GET /api/stations` → Estações
- `GET /api/vehicles` → Veículos
- `GET /api/metrics` → Métricas

---

**🚀 Versão**: 0.2.0  
**📅 Data**: 2025-11-23  
**✨ Status**: ✅ PRONTO PARA UNITY

---

**💡 Dica**: Deixe a API rodando em um terminal e abra outro para trabalhar no Unity!

