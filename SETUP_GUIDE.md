# Guía Rápida: Aplicar Mejoras de Controles Móviles
# Quick Guide: Apply Mobile Controls Improvements

## Paso 1: Abrir Unity / Step 1: Open Unity

1. Abre el proyecto Ninja_Gaiden en Unity Editor
2. Espera a que termine de compilar los scripts

## Paso 2: Aplicar Mejoras a Nivel1 / Step 2: Apply Improvements to Nivel1

### Opción A: Usar Herramienta Automática (Recomendado)

1. Abre la escena: `Assets/Scenes/Nivel1.unity`
2. En el menú superior: **Tools → Mobile Controls → Improve Mobile UI in Current Scene**
3. Verifica en la consola: "✓ Improved X mobile controls!"
4. En el menú: **Tools → Mobile Controls → Add Mobile Controls Setup to Canvas**
5. Guarda la escena: `Ctrl+S` (Windows) o `Cmd+S` (Mac)

### Opción B: Manual

1. Abre `Assets/Scenes/Nivel1.unity`
2. En la jerarquía, selecciona el GameObject `Canvas`
3. En el Inspector, click "Add Component"
4. Busca y agrega: `Mobile Controls Setup`
5. En el campo `Joystick`, arrastra el "Floating Joystick" desde la jerarquía
6. Marca `Force Show Controls` para testing en editor
7. Selecciona cada botón (ButtonA, ButtonY, etc.):
   - Opcional: Agrega componente `Enhanced Mobile Button`
   - Ajusta `Button Alpha` a 0.85
   - Ajusta `Minimum Size` a (120, 120)
8. Guarda la escena

## Paso 3: Testear / Step 3: Test

### En Editor:
1. Click en Play (▶)
2. Los controles deberían ser visibles
3. Usa el mouse para interactuar

### En Dispositivo Móvil:
1. File → Build Settings
2. Selecciona Android o iOS
3. Click "Build and Run"
4. Prueba en dispositivo real

## Paso 4: Crear Nueva Escena con Controles (Opcional)

Si quieres crear una nueva escena con controles móviles desde cero:

1. Crea nueva escena: File → New Scene
2. Menú: **Tools → Mobile Controls → Create Mobile UI Setup**
3. En la ventana que aparece:
   - System Type: Selecciona según tu sistema (New StateMachine o Old Player)
   - Marca los controles que quieres incluir
   - Ajusta posiciones si es necesario
   - Click "Create Mobile UI"
4. Configura eventos de botones:
   - Para New StateMachine: Ya están conectados a InputBuffer
   - Para Old Player.cs:
     - Selecciona cada botón
     - En Button (Script) → On Click ()
     - Click "+"
     - Arrastra el Player GameObject
     - Selecciona función: Player.SaltoTactil (para Jump) o Player.AtaqueTactil (para Attack)
5. Agrega tu Player prefab a la escena
6. Guarda la escena

## Verificación / Verification

Checklist para verificar que todo funciona:

- [ ] Los botones son visibles (no muy transparentes)
- [ ] Los botones son suficientemente grandes (≥100px)
- [ ] El joystick aparece y responde al mouse/touch
- [ ] Los botones responden al click/touch
- [ ] En dispositivo móvil: se siente vibración al presionar botones
- [ ] En desktop: los controles se ocultan automáticamente (si autoHideOnDesktop = true)
- [ ] Los controles no interfieren con el gameplay

## Solución de Problemas / Troubleshooting

### Los scripts no aparecen en menú Tools:
- Asegúrate de que los archivos .meta existen
- Recompila: Assets → Reimport All
- Reinicia Unity Editor

### Error de compilación:
- Verifica que todos los scripts están en las carpetas correctas:
  - `Assets/Scripts/Editor/` para MobileControlsEditor y CreateMobileUISetup
  - `Assets/Scripts/MobileUI/` para MobileControlsSetup, EnhancedMobileButton, etc.
  - `Assets/Scripts/` para MobileControls.cs
- Cierra y reabre Unity

### Los botones no responden:
- Verifica que existe un EventSystem en la escena
- Verifica que el Canvas tiene un GraphicRaycaster
- Verifica que los botones tienen el componente Button

### Los controles no aparecen en móvil:
- Verifica que el Canvas y sus hijos están activos
- En MobileControlsSetup, desmarca `Auto Hide On Desktop`
- Marca `Force Show Controls`

## Estructura Final Esperada

```
Canvas
├── MobileControls (GameObject con RectTransform)
│   ├── Floating Joystick (Prefab)
│   └── Buttons
│       ├── ButtonY (Jump) → Player.SaltoTactil
│       ├── ButtonA (Attack) → Player.AtaqueTactil
│       ├── ButtonB (Opcional)
│       └── ButtonX (Opcional)
└── ... (otros UI elementos)

Canvas (GameObject) debería tener:
- Canvas (componente)
- Canvas Scaler (componente)
- Graphic Raycaster (componente)
- Mobile Controls Setup (componente) ← NUEVO
```

## Características Implementadas

✅ Detección automática de plataforma
✅ Botones con tamaño optimizado para móvil (≥100px)
✅ Opacidad mejorada (0.85)
✅ Feedback visual (escala al presionar)
✅ Feedback háptico (vibración en móvil)
✅ Herramientas de Unity Editor
✅ Sistema compatible con Player.cs antiguo
✅ Sistema compatible con nuevo StateMachine + InputBuffer
✅ Documentación completa

## Próximos Pasos Recomendados

1. Testear en dispositivo Android/iOS real
2. Ajustar posiciones según feedback de usuarios
3. Considerar agregar botón de pausa
4. Agregar tutorial/hints para nuevos jugadores
5. Optimizar tamaño de assets para móvil

## Recursos Adicionales

- **Documentación completa**: `MOBILE_CONTROLS.md`
- **Scripts**: `Assets/Scripts/MobileUI/`
- **Editor Tools**: `Assets/Scripts/Editor/`
- **Prefabs de Joystick**: `Assets/Joystick Pack/Prefabs/`
- **Sprites de botones**: `Assets/sprites/UIButtons/XBOX BUTTONS - Premium Assets/`

## Soporte

Si encuentras problemas:
1. Revisa la consola de Unity para errores
2. Verifica que todos los scripts se compilaron correctamente
3. Lee `MOBILE_CONTROLS.md` para más detalles
4. Asegúrate de tener todas las dependencias (Joystick Pack, etc.)
