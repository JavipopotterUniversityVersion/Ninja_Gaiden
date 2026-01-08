# Quick Start Guide - Items System

## Rápida configuración para probar el sistema de items

### Paso 1: Configurar el Jugador

1. **Abre Unity y la escena principal**
   - Assets/Scenes/Nivel1.unity (o tu escena de juego)

2. **Selecciona el GameObject del Player**

3. **Añade ItemInventory:**
   - Click en "Add Component"
   - Busca y añade "ItemInventory"

4. **Configura el StateMachine:**
   - En el componente StateMachine, busca el diccionario "States"
   - Añade una nueva entrada:
     - Key: `PLAYER_USE_ITEM`
     - Value: Arrastra el componente UseItemState (lo añadiremos a continuación)

5. **Añade UseItemState:**
   - Con el Player seleccionado, "Add Component"
   - Busca y añade "UseItemState"
   - En UseItemState, arrastra el ItemInventory al campo "Item Inventory"

### Paso 2: Configurar Input

1. **Abre InputBuffer GameObject en la escena**

2. **Añade entrada USE_ITEM:**
   - En el componente InputBuffer, busca "Input Buffer" (diccionario)
   - Añade nueva entrada:
     - Key: `USE_ITEM`
     - Duration: `0.2`

3. **Configura el botón:**
   
   **Para teclado (Unity Input Manager):**
   - Edit → Project Settings → Input Manager
   - Duplica un input existente
   - Cambia Name a: `USE_ITEM`
   - Positive Button: `e` (o tu tecla preferida)

   **Para móvil:**
   - Crea un UI Button en el Canvas
   - Añade componente "EnhancedTouchScreenButtonInput"
   - Input Name: `USE_ITEM`

### Paso 3: Crear un Pickup de Shuriken

1. **Crear GameObject:**
   - Hierarchy → Create Empty
   - Nombre: "ShurikenPickup"

2. **Añadir visual:**
   - Add Component → Sprite Renderer
   - Asigna un sprite (puedes usar un círculo o estrella temporalmente)
   - Color: Gris/Plateado

3. **Añadir físicas:**
   - Add Component → Circle Collider 2D
   - Marca "Is Trigger" ✓
   - Radius: 0.3

4. **Añadir lógica de item:**
   - Add Component → "ShurikenItem"
   - Add Component → "ItemPickup"
   - En ItemPickup, arrastra ShurikenItem al campo "Item Behaviour"

5. **Posicionar:**
   - Coloca el pickup cerca del player en la escena
   - Transform Position: Cerca del spawn del jugador

### Paso 4: Crear un Pickup de Body Possession

1. **Repetir pasos del Shuriken pero:**
   - Nombre: "BodyPossessionPickup"
   - Color del sprite: Púrpura/Morado
   - En vez de ShurikenItem, usa "BodyPossessionItem"

### Paso 5: Configurar Enemigos

Para que los enemigos puedan ser poseídos:

1. **Selecciona un enemigo en la escena**

2. **Verifica que tiene:**
   - Tag: `Enemy` (importante!)
   - Componente HealthHandler
   - Componente MovementController (si no, se añadirá automáticamente)

3. **Si no tiene MovementController:**
   - Add Component → "MovementController"
   - Speed: 3-5 (ajusta según prefieras)

### Paso 6: Probar el Sistema

1. **Guardar la escena** (Ctrl+S)

2. **Play** (Ctrl+P)

3. **Probar Shuriken:**
   - Mueve al jugador sobre el pickup de shuriken
   - Presiona la tecla 'E' (o el botón configurado)
   - Un proyectil debería salir en la dirección que mires
   - El proyectil daña enemigos al contacto

4. **Probar Body Possession:**
   - Recoge el pickup de posesión
   - Acércate a un enemigo (menos de 3 unidades)
   - Presiona 'E'
   - Deberías controlar al enemigo con las teclas de movimiento
   - Si el enemigo muere, vuelves a tu cuerpo original

---

## Controles por Defecto

- **Movimiento:** WASD / Flechas / Joystick móvil
- **Salto:** Espacio / Botón Jump
- **Ataque:** Click izquierdo / Botón Attack
- **Usar Item:** E / Botón USE_ITEM

---

## Solución Rápida de Problemas

### "El item no hace nada al presionar E"
- ✓ Verifica que ItemInventory está en el Player
- ✓ Asegúrate de haber recogido un item primero
- ✓ Revisa que USE_ITEM está en InputBuffer
- ✓ Confirma que UseItemState está en el diccionario States

### "No puedo recoger items"
- ✓ Verifica que el Player tiene Tag "Player"
- ✓ Asegúrate que el pickup tiene un Collider2D con Is Trigger marcado
- ✓ Confirma que ItemPickup tiene el item asignado en Item Behaviour

### "No puedo poseer enemigos"
- ✓ El enemigo debe tener Tag "Enemy"
- ✓ Debes estar a menos de 3 unidades de distancia
- ✓ El enemigo debe estar vivo (Health > 0)

### "El shuriken no aparece"
- ✓ Verifica la consola de Unity por errores
- ✓ Si usas un prefab, asegúrate que está asignado en ShurikenItem
- ✓ Si no hay prefab, el sistema creará uno automáticamente

---

## Configuración Avanzada

### Ajustar Shuriken

En el componente ShurikenItem puedes configurar:
- **Shuriken Prefab:** Prefab personalizado del proyectil
- **Cooldown:** Tiempo entre usos (default: 0.5s)

En el script ShurikenProjectile.cs puedes cambiar:
- **Speed:** Velocidad del proyectil (default: 10)
- **Damage:** Daño a enemigos (default: 10)
- **Lifetime:** Tiempo antes de destruirse (default: 5s)

### Ajustar Body Possession

En el script BodyPossessionItem.cs puedes cambiar:
- **possessionRange:** Distancia máxima para poseer (default: 3.0f)
  - Línea 16: `private float possessionRange = 3f;`

---

## Crear Items Personalizados

```csharp
using UnityEngine;

public class MiItemPersonalizado : MonoBehaviour, IItem
{
    public string GetItemName() 
    { 
        return "Mi Item"; 
    }
    
    public bool CanUse() 
    { 
        return true; // o tu lógica de validación
    }
    
    public void Use(GameObject user)
    {
        // Tu código aquí
        Debug.Log("¡Item usado!");
        
        // Ejemplo: Curar al jugador
        HealthHandler health = user.GetComponent<HealthHandler>();
        if (health != null)
        {
            // health.Heal(20); // Si tienes método Heal
        }
    }
}
```

---

## Siguiente Nivel

Una vez que el sistema básico funciona, puedes:

1. **Crear más tipos de items:**
   - Items de curación
   - Items de velocidad temporal
   - Items de invisibilidad
   - Bombas o explosivos

2. **Mejorar visuales:**
   - Sprites personalizados para pickups
   - Efectos de partículas al recoger
   - Animaciones para items
   - UI mostrando item actual

3. **Ampliar mecánicas:**
   - Múltiples slots de items
   - Items con usos limitados
   - Items que se pueden tirar
   - Sistema de crafteo

4. **Mejorar posesión:**
   - Poder usar ataques del enemigo poseído
   - Límite de tiempo de posesión
   - Efectos visuales de posesión
   - Poder cambiar entre enemigos sin volver al cuerpo

---

¡Disfruta experimentando con el sistema de items! 🎮✨
