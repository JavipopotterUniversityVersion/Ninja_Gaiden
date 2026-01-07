# Controles para Móvil - Mobile Controls Setup

Este documento explica cómo configurar y usar los controles móviles en Ninja Gaiden.

## Características / Features

### Sistema de Controles Móviles
- **Joystick Virtual**: Control de movimiento con joystick flotante
- **Botones de Acción**: Botones táctiles para saltar y atacar
- **Detección Automática**: Se muestran solo en dispositivos móviles
- **Optimización**: Tamaño y visibilidad optimizados para pantallas táctiles

## Configuración Actual / Current Setup

### Escena Nivel1.unity

La escena principal ya incluye controles móviles:

1. **Joystick Flotante** (Floating Joystick)
   - Ubicación: Esquina inferior izquierda
   - Función: Control de movimiento del personaje
   - Tipo: Floating (aparece donde tocas)

2. **Botones de Acción** existentes:
   - **ButtonY**: Salto (Jump) - Llama a `Player.SaltoTactil()`
   - **ButtonA**: Ataque (Attack) - Llama a `Player.AtaqueTactil()`
   - **ButtonB**: Agarrar enemigo
   - **ButtonX**: Acción adicional
   - **ButtonCORRER**: Correr

## Mejoras Implementadas / Implemented Improvements

### 1. Scripts de Gestión

#### MobileControlsSetup.cs
Script que optimiza automáticamente los controles móviles:

**Características:**
- Ajusta el tamaño mínimo de botones (120x120px) para mejor usabilidad
- Aumenta la opacidad de botones para mejor visibilidad (alpha: 0.85)
- Detecta automáticamente la plataforma (móvil/escritorio)
- Oculta controles en escritorio automáticamente

**Uso:**
1. Abre la escena en Unity Editor
2. Selecciona el GameObject "Canvas" en la jerarquía
3. Agrega el componente "Mobile Controls Setup"
4. El script se ejecutará automáticamente al iniciar

#### MobileControls.cs
Script básico para mostrar/ocultar controles móviles:

**Uso:**
- Asigna los contenedores de joystick y botones
- Configura `autoDetectMobile` = true para detección automática
- Usa `forceShowControls` = true para testing en editor

### 2. Recomendaciones de Diseño

#### Tamaño de Botones
- **Mínimo recomendado**: 120x120 píxeles
- **Botones actuales**: ~110x53 píxeles (pueden ser pequeños)
- **Solución**: El script MobileControlsSetup ajusta automáticamente

#### Posicionamiento
Configuración recomendada para pantallas móviles:

**Joystick** (Inferior Izquierda):
- Anchor: Bottom-Left
- Posición: (150, 150) desde esquina
- Tamaño: 150x150

**Botón de Salto** (Inferior Derecha):
- Anchor: Bottom-Right  
- Posición: (-150, 150) desde esquina
- Tamaño: 120x120

**Botón de Ataque** (Derecha del Salto):
- Anchor: Bottom-Right
- Posición: (-150, 280) desde esquina
- Tamaño: 120x120

#### Visibilidad
- **Alpha recomendado**: 0.7 - 0.9
- **Alpha actual**: 0.66 (un poco transparente)
- **Solución**: Ajustar en el script o en Inspector

### 3. Integración con Sistema de Input

Los controles móviles se integran con dos sistemas:

#### Sistema Antiguo (Player.cs)
```csharp
// En Player.cs línea 167-168
float horizontalInput = Input.GetAxisRaw("Horizontal") + joy.Horizontal;
float verticalInput = Input.GetAxisRaw("Vertical") + joy.Vertical;

// Botones llaman métodos directamente:
// ButtonY -> Player.SaltoTactil()
// ButtonA -> Player.AtaqueTactil()
```

#### Sistema Nuevo (StateMachine + InputBuffer)
```csharp
// En IdleState.cs y MoveState.cs
float horizontal = Input.GetAxisRaw("Horizontal") + joystick.Horizontal;
float vertical = Input.GetAxisRaw("Vertical") + joystick.Vertical;

// Los botones pueden usar TouchScreenButtonInput:
// Envía input al InputBuffer
// InputBuffer notifica al StateMachine
```

## Cómo Agregar Controles a Nueva Escena / How to Add Controls to New Scene

### Método 1: Usar Prefabs Existentes

1. Crea un Canvas UI:
   - GameObject → UI → Canvas
   - Canvas Scaler: Scale With Screen Size
   - Reference Resolution: 1920x1080

2. Agrega el Joystick:
   - Arrastra `Assets/Joystick Pack/Prefabs/Floating Joystick.prefab` al Canvas
   - Posiciona en esquina inferior izquierda
   - Ajusta tamaño: 150x150

3. Crea Botones de Acción:
   - GameObject → UI → Button
   - Nómbralos: "ButtonJump", "ButtonAttack"
   - Posiciona en esquina inferior derecha
   - Tamaño: 120x120

4. Configura Eventos:
   - Selecciona ButtonJump
   - En componente Button, evento OnClick:
     - Arrastra el Player GameObject
     - Selecciona función: Player.SaltoTactil
   - Repite para ButtonAttack → Player.AtaqueTactil

5. Agrega MobileControlsSetup:
   - Selecciona Canvas
   - Add Component → Mobile Controls Setup
   - Asigna el Joystick en el inspector
   - Marca "Force Show Controls" para testing

### Método 2: Copiar desde Nivel1

1. Abre Nivel1.unity
2. Encuentra el Canvas con los controles
3. Copia los siguientes GameObjects:
   - Floating Joystick
   - ButtonY (Salto)
   - ButtonA (Ataque)
4. Pégalos en tu nueva escena

## Testing

### En Unity Editor
1. Marca "Force Show Controls" en MobileControlsSetup
2. Play Mode
3. Los controles deberían ser visibles
4. Usa el mouse para simular toques

### En Dispositivo Móvil
1. Build Settings → Android o iOS
2. Build and Run
3. Los controles aparecerán automáticamente
4. Verifica tamaño y posición adecuados

## Solución de Problemas / Troubleshooting

### Controles No Visibles
- Verifica que el Canvas esté activo
- Revisa que "Force Show Controls" esté marcado en editor
- Comprueba que los GameObjects no estén desactivados

### Botones No Responden
- Verifica que haya un EventSystem en la escena
- Comprueba que el Canvas tenga un Raycaster
- Revisa que los botones tengan el componente Button

### Joystick No Funciona
- Asegúrate de que el script del Player tenga referencia al joystick
- Verifica que el componente Joystick esté presente
- En IdleState/MoveState, confirma que `joystick` esté asignado

## Notas Adicionales

- Los controles se ocultan automáticamente en desktop (excepto si se marca forceShowControls)
- El sistema actual usa el Player.cs antiguo en Nivel1
- Para nuevas escenas, considera usar el sistema StateMachine + InputBuffer
- Los sprites de botones están en `Assets/sprites/UIButtons/XBOX BUTTONS - Premium Assets/`

## Próximos Pasos Sugeridos

1. Aumentar tamaño de botones en Nivel1 para mejor UX móvil
2. Ajustar alpha de botones a 0.8-0.9 para mejor visibilidad
3. Agregar feedback visual en botones (scaling, color)
4. Considerar vibración háptica en dispositivos móviles
5. Testear en diferentes tamaños de pantalla móvil
