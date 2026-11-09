# Helix Jump — Actividades 4.1 y 4.2

Implementación de los capítulos 1 a 4 de LuisCanary, adaptada a Unity **6000.6.7f1**, URP e Input System.

## Abrir y probar

1. Abre `Assets/HelixJump/Scenes/HelixJump.unity`.
2. En Game selecciona la resolución **Helix Jump Portrait (1080 × 1920)**, o cualquier proporción vertical 9:16.
3. Pulsa Play. La bola verde cae y rebota; Score empieza en cero y Best carga el récord local.
4. Mantén pulsado el botón izquierdo del ratón sobre Game y arrastra horizontalmente para girar la torre. En móvil, el controlador lee el toque principal mediante Input System; pendiente de validación en dispositivo físico.
5. Alinea los huecos de las dos plataformas con la bola para descender hasta la meta dorada. No necesitas desactivar piezas manualmente.
6. Al tocar la meta se escribe **Pasamos de nivel** en Console. No cambia de escena: ese desarrollo pertenece a capítulos posteriores.
7. Detén Play y vuelve a iniciarlo: Score se reinicia y Best se conserva. El récord local puede incluir puntuaciones obtenidas durante las pruebas de desarrollo.

**Regla de los capítulos 3 y 4:** se suma un punto por cada rebote aceptado, incluso sobre la misma plataforma. El bloqueo de contactos duplicados impide dos puntos por un mismo rebote. La puntuación por superar plataformas, obstáculos letales, dificultad progresiva, victoria/derrota, generación de niveles, cambio real de escena y menús NO se implementan todavía.

## Implementado

- Modelo `Assets/Models/HelixPart.FBX` y prefab originales importados por el estudiante.
- Prefab reutilizable `Assets/HelixJump/Prefabs/HelixLevel.prefab`, compuesto por sectores cada 30° y MeshColliders con su malla asignada.
- Dos plataformas con huecos, torre central, meta inferior dorada y bola con SphereCollider/Rigidbody.
- Rebote mediante impulso de 3 N·s con masa de 1 kg, reinicio de velocidad y bloqueo de contactos duplicados durante 0.2 s.
- Bola restringida a Y, sin rotación. Detección continua e interpolación.
- Seguimiento de cámara en Y con separación inicial constante, manteniendo X/Z. Actualización en LateUpdate.
- Material físico sin rebote adicional para no acumular impulsos; materiales gráficos compatibles con URP.
- Canvas adaptable a resolución con **Helix Jump**, **Score** y **Best**. Texto marfil con contorno oscuro para mantener contraste durante el descenso.
- Paleta navideña: bola verde esmeralda, plataformas rojo arándano/blanco cálido, torre verde pino, meta dorada y fondo menta. El título y los contadores usan marfil con contorno verde oscuro.
- Orientación vertical y escena incluida en la lista de compilación. La plataforma Android ya estaba activa y sus módulos estaban disponibles.

## Archivos

- `Assets/HelixJump/Scripts/BallController.cs`: rebote y restricciones.
- `Assets/HelixJump/Scripts/CameraFollow.cs`: seguimiento vertical.
- `Assets/HelixJump/Scripts/HelixController.cs`: arrastre horizontal con ratón/toque usando Input System; sensibilidad ajustable en Inspector.
- `Assets/HelixJump/Scripts/GameManager.cs`: singleton, puntuación y récord persistente. Clave exclusiva de PlayerPrefs: `HelixJump.HighScore`.
- `Assets/HelixJump/Scripts/UIManager.cs`: textos Score/Best.
- `Assets/HelixJump/Scripts/GoalController.cs`: detección de bola en meta y aviso, sin transición real.
- `Assets/HelixJump/Scenes/HelixJump.unity`: escena principal.
- `Assets/HelixJump/Prefabs/`: plataforma y bola reutilizables.
- `Assets/HelixJump/Materials/` y `Physics/`: paleta URP y material físico.
- `Assets/HelixJump/Tests/Editor/`: pruebas de rebote, cámara y configuración de escena.
- `Assets/HelixJump/Editor/TutorialSceneBuilder.cs`: construcción inicial. No sobrescribe una escena existente; no es necesario ejecutarlo para jugar.
- `Assets/HelixJump/Editor/Chapter34Setup.cs`: ampliación inicial de la escena. Ya aplicada; rechaza duplicarla y no es necesario ejecutarla para jugar.

Los assets originales importados y SampleScene se conservaron. No se ejecutaron comandos Git ni operaciones GitHub.

## Validación realizada

- Nueve pruebas EditMode aprobadas: giro Y y reinicio del arrastre, suma única por rebote, récord persistente entre sesiones, actualización Score/Best, meta filtrada y aviso único, conexión de escena y tres pruebas de regresión de capítulos 1/2.
- Play Mode: rebotes repetidos sobre la unión de dos sectores.
- Play Mode: apertura temporal de las dos piezas bajo la bola y descenso hasta la plataforma inferior, con nuevo rebote y separación de cámara de 3 unidades.
- Capítulos 3/4 en Play Mode: eventos MouseState de Input System hicieron girar la torre, permitieron descender por ambas plataformas y alcanzar la meta; GoalReached=true, CurrentLevel=0 y Score igual a BounceCount. Se conservaron X/Z de bola y separación Y de cámara.
- Reinicio real de Play: Best recuperó el récord de la sesión anterior mientras Score volvió a empezar. Las pruebas EditMode comprueban el valor inicial cero y restauran la clave anterior de PlayerPrefs.
- El ensayo sintético Touchscreen en Editor no fue concluyente; no se considera validación táctil completada. El soporte de lectura táctil está implementado, pero debe probarse en un dispositivo Android antes de afirmar compatibilidad física.
- Captura real de Game View en 1080 × 1920, revisada visualmente.

No se generó APK ni se probó en un dispositivo Android físico. No se afirma validación de rendimiento móvil.

## Tutoriales

1. Crear tu primer Videojuego con Unity/HelixJump: https://youtu.be/QYUgb5pHi9c
2. Crea tu primer Videojuego para Android con Unity/HelixJump/Fisicas Bola/2-Cap: https://youtu.be/VHLz-UHErnA
3. Movimiento Helix y Score/3-Cap: https://youtu.be/-iNJYemon3Y
4. Score y HighScore/4-Cap: https://youtu.be/NSQ8D0i2_Sw
