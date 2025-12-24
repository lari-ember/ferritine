# Posicionamento de Estações (API → Unity)

Este documento descreve o fluxo completo de posicionamento das estações (e por extensão veículos e agentes) na Ferritine VU: como as coordenadas chegam do backend, como são mapeadas no mundo 3D do Unity e onde procurar/alterar o código quando for necessário.

Sumário
- Visão geral do fluxo
- Mapeamento de eixos (API → Unity)
- Trechos de código importantes
- Exemplo concreto
- Como validar no Unity
- Problemas comuns e soluções
- Extensões e melhorias possíveis

---

## 1. Visão geral do fluxo

1. O backend expõe uma API que fornece dados de entidades (estações, veículos, agentes). Cada estação vem com coordenadas 2D: `x` e `y` (inteiros).
2. O cliente Unity recebe esses dados no `WorldController` (método `UpdateWorld`) e converte as coordenadas 2D em uma posição 3D do Unity.
3. A conversão padrão usada no projeto é:

```text
Unity position = new Vector3(api.x, 0, api.y)
```

ou seja, `api.x` → `position.x`, `api.y` → `position.z` e `position.y` é a altura (fixa em 0, chão).

## 2. Mapeamento de eixos (API → Unity)

| API (2D)      | Unity (3D)       | Observação                 |
|---------------|------------------|----------------------------|
| `x` (int)     | `position.x`     | eixo Leste/Oeste           |
| `y` (int)     | `position.z`     | eixo Norte/Sul             |
| (nenhum)      | `position.y = 0` | altura fixa no chão        |

> Raciocínio: API fornece coordenadas top-down (plano XY). No Unity, convencionamos Y como vertical, então o `y` do backend é mapeado para `z` no Unity.

## 3. Trechos de código (onde olhar)

- Modelo de dados (recebido do backend):
  - `Assets/Scripts/API/Models/StationData.cs`

```csharp
[Serializable]
public class StationData
{
    public string id;
    public string name;
    public int x; // coordenada X do mapa
    public int y; // coordenada Y do mapa
    // ... outros campos
}
```

- Conversão e spawn (no `WorldController`):
  - `Assets/Scripts/Controllers/WorldController.cs`

```csharp
// Ao criar / obter a estação do pool
Vector3 worldPos = new Vector3(data.x, 0, data.y);
stationObj.transform.position = worldPos;
```

- Uso no Teleport UI (lista de destinos e preview):
  - `Assets/Scripts/UI/TeleportSelectorUI.cs`

```csharp
// Ao montar lista de destinos (carregado do WorldController)
position = new Vector3(station.x, 0, station.y);
// Para preview/teleporte base
cameraController.PreviewLocation(position);
```

## 4. Exemplo concreto

Se a API retorna:

```json
{ "id": "station_central", "name": "Estação Central", "x": 100, "y": 250 }
```

Então, no Unity:

```csharp
var pos = new Vector3(100, 0, 250);
// pos.x = 100 (Leste), pos.y = 0 (chão), pos.z = 250 (Norte)
```

Esse `pos` é usado para:
- posicionar o GameObject da estação
- construir a lista de destinos no TeleportSelectorUI
- servir de base para teleporte / preview

## 5. Como validar no Unity (passo-a-passo)

1. Abra a cena com `WorldController`.
2. No Inspector do `WorldController`, confirme que os prefabs (station/vehicle/agent) estão corretamente atribuídos.
3. Dê Play.
4. Abra o console (Window → General → Console) e procure logs que adicionamos:
   - `[WorldController] UpdateWorld chamado - Timestamp: ...`
   - `[WorldController] Stations: N` (N > 0)
   - `[TeleportSelectorUI] Loaded X real stations from WorldController`
   - `[TeleportSelectorUI] Selected: NAME (station) at (x, 0, z)`
5. Na Hierarquia, expanda `StationsContainer` (criado por `WorldController`) e veja as estações instanciadas nas posições esperadas.
6. Se necessário, selecione uma estação e pressione `F` para focar a cena e verificar suas coordenadas no Inspector.

## 6. Problemas comuns e soluções

- "Estação aparece em outra posição ou todas em linha":
  - Verifique se o `WorldController` está usando `new Vector3(data.x, 0, data.y)` (e não `new Vector3(data.x, 0, data.x)` ou outra combinação).
  - Verifique se os dados do backend estão corretos (x/y não invertidos). Logs do backend ou sample JSON ajudam.

- "UI de Teleporte mostra coordenadas diferentes":
  - Confirme que o `TeleportSelectorUI` está carregando destinos do `WorldController` (não usando valores hardcoded).

- "Teleporte leva a posição aleatória":
  - Verifique `CalculateTeleportPosition()` — não deve usar `Random.Range` para offset que precisa ser reproduzível. Use offset determinístico ou pequeno radius fixo.

- "Objetos posicionados, mas não visíveis":
  - Verifique se o objeto tem `Renderer` ativo, se está em layer visível e se a câmera está olhando para a região correta.

## 7. Extensões e melhorias sugeridas

- Se o terreno tiver elevação, backend pode enviar `elevation` ou `height` para mapear para `position.y`.

- Oferecer configuração central de mapeamento em `WorldController` (uma função `MapFromAPI(int x, int y) => Vector3`) para evitar duplicação.

- Se quiser evitar sobreposição visual, aplicar um deslocamento determinístico por entidade (hash-based) como já implementado para teleporte.

- Documentar também o sistema de coordenadas no README do projeto para novos desenvolvedores.

---

Se quiser, eu já crio um arquivo similar também no diretório `docs/` em português e/ou em inglês com links para os trechos de código (e.g., `WorldController.cs`, `TeleportSelectorUI.cs`) — quer que eu crie esse arquivo agora?
