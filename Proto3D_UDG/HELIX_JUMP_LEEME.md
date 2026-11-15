# Helix Jump — Actividades 4.1 a 4.5

Implementación de los capítulos 1 a 10 de LuisCanary, adaptada a Unity **6000.6.7f1**, URP e Input System.

## Abrir y probar

1. Abre `Assets/HelixJump/Scenes/MainMenu.unity` para comenzar desde el menú (la escena de juego sigue siendo `HelixJump.unity`).
2. En Game selecciona la resolución **Helix Jump Portrait (1080 × 1920)**, o cualquier proporción vertical 9:16.
3. Pulsa Play y después **Jugar** en el menú. La bola verde cae y rebota; Score empieza en cero y Best carga el récord local.
4. Mantén pulsado el botón izquierdo del ratón sobre Game y arrastra horizontalmente para girar la torre. En móvil, el controlador lee el toque principal mediante Input System; pendiente de validación en dispositivo físico.
5. Alinea los huecos de la plataforma superior y las tres plataformas generadas con la bola para descender hasta la meta dorada. Los huecos y peligros de las plataformas generadas cambian al volver a iniciar Play.
6. Al tocar la meta avanzas al siguiente Stage, conservando Score y regresando la bola al inicio. Al agotar los tres Stages se recarga el último sin índices fuera de rango. Los Stages comparten la misma escena; SceneManager se utiliza del menú al juego.
7. Detén Play y vuelve a iniciarlo: Score se reinicia y Best se conserva. El récord local puede incluir puntuaciones obtenidas durante las pruebas de desarrollo.

**Regla actual del capítulo 5:** +1 al atravesar cada plataforma mediante un trigger, una vez por intento. Rebotar ya no suma puntos. Tocar una pieza roja reinicia Score y posición/velocidad de la bola, rearma triggers y meta, y conserva Best. Marfil identifica superficies seguras; rojo, peligros. No se añaden vidas, menús ni pantallas de derrota.

## Implementado

- Modelo `Assets/Models/HelixPart.FBX` y prefab originales importados por el estudiante.
- Prefab reutilizable `Assets/HelixJump/Prefabs/HelixLevel.prefab`, compuesto por sectores cada 30° y MeshColliders con su malla asignada.
- Plataforma superior fija y tres plataformas intermedias generadas desde Stage, torre central, meta inferior dorada y bola con SphereCollider/Rigidbody.
- Rebote mediante impulso de 4 N·s con masa de 1 kg, reinicio de velocidad y bloqueo de contactos duplicados durante 0.2 s.
- Bola restringida a Y, sin rotación. Detección continua e interpolación.
- Seguimiento de cámara en Y con separación inicial constante, manteniendo X/Z. Actualización en LateUpdate.
- Material físico sin rebote adicional para no acumular impulsos; materiales gráficos compatibles con URP.
- Canvas adaptable a resolución con **Helix Jump**, **Score** y **Best**. Texto marfil con contorno oscuro para mantener contraste durante el descenso.
- Paleta navideña: bola verde esmeralda, plataformas marfil con sectores peligrosos rojos, torre verde pino, meta dorada y fondo menta. El título y los contadores usan marfil con contorno verde oscuro.
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

## Validación histórica de capítulos 1 a 4

Estos resultados corresponden a versiones anteriores; la regla de puntuación por rebote fue sustituida por el capítulo 5. La validación actual se detalla al final.

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
5. Score y DeathPart/5-Cap: https://youtu.be/0asOPNehwHY
6. Lógica Levels/6-Cap: https://youtu.be/tx5PDevroCE

## Estado histórico al finalizar la actividad 4.3 — capítulos 5 y 6

La actividad 4.4 descrita al final sustituye el LoadStage vacío por generación y aplicación de colores. No ejecutar Chapter56Setup sobre la escena actual para continuar jugando.

Se consultaron las transcripciones completas de ambos apoyos. Alcance limitado a los tutoriales por decisión expresa del estudiante: no adelantar mecánicas de capítulos posteriores.

### Componentes nuevos
- `PassScorePoint.cs`: BoxCollider trigger; filtra la bola, suma +1 una vez y se rearma al reiniciar. Retirada la puntuación por rebote de BallController.
- `DeathPart.cs`: peligro rojo al activarse; MaterialPropertyBlock evita cambiar materiales compartidos de otras piezas.
- `BallController.ResetBall()`: vuelve a la posición inicial y limpia la velocidad física. La detección de peligro tiene prioridad sobre el rebote.
- `GameManager.RestartLevel()`: Score a cero, restablecimiento de bola, rearme de triggers/meta y actualización UI. Conserva la clave `HelixJump.HighScore`.
- `Prefabs/HelixLevel_Chapter56.prefab`: prefab propio con puntuación por paso y peligro; preserva los recursos originales importados.
- `Editor/Chapter56Setup.cs`: configuración ya aplicada de las dos plataformas y sus peligros. No es necesario ejecutarla para jugar; puede reemplazar ajustes manuales de dichas plataformas.

### Stage: datos editables, no generador
Crea assets desde **Assets > Create > Helix Jump > Stage**. En `Stages/Stage_Example.asset` hay 12 secciones con `partCount=11` y `deathCount=0`.

`Stage` contiene colores `backgroundColor`, `helixColor`, `ballColor` (blancos por defecto) y una lista de `Level` serializable. Cada sección expone `partCount` (1–11, inicial 11) y `deathCount` (0–11, inicial 1). Son cantidades de sectores por anillo, no un límite de 11 anillos por stage.

`HelixController` conserva el arrastre y añade referencias a plataforma superior, meta, prefab, stages y plataformas. Calcula la distancia vertical disponible (6.1 en esta escena). **LoadStage(int) permanece vacío y su llamada inicial comentada**, tal como termina el capítulo 6. Los datos de Stage aún no generan piezas ni cambian colores del escenario.

No se implementan generación infinita/continua, dificultad progresiva, transiciones reales ni aplicación automática de colores de Stage. Esta preparación no se presenta como generación terminada.

### Cómo verificar lo nuevo
1. Pulsa Play y rebota en marfil: Score no aumenta.
2. Gira la torre para atravesar un hueco: Score aumenta en uno.
3. Toca un sector rojo: bola al inicio y Score a cero. Best se conserva; puedes puntuar nuevamente al atravesar las plataformas.
4. Selecciona Stage_Example en Project para editar sus secciones y colores; no altera todavía la escena.

### Validación actual
Suite Unity Pipeline, filtro HelixJump, modo EditMode: **12/12 aprobadas**. Incluye una UnityTest que entra realmente a Play Mode y verifica OnTriggerEnter y colisión DeathPart con física: suma por paso, rearme, reinicio de posición/velocidad, Score, conservación de Best y restablecimiento de meta. El ensayo usa objetos aislados lejos de la torre e inmoviliza temporalmente la bola original; nada de eso queda guardado en la escena.

Se verificaron además el asset Stage de 12 secciones, referencias del Inspector, dos triggers y dos peligros activos, prefab propio y regresiones de controles, cámara, UI y persistencia. Las pruebas deben restaurar solamente la clave concreta de PlayerPrefs, sin DeleteAll. Captura real de Game View en 1080 × 1920 revisada visualmente. Escena guardada, Play detenido y override Run In Background desactivado al terminar.

No se generó APK ni se validó Android físico. Git/GitHub y la entrega están bajo control del usuario. El ZIP es respaldo de fuentes reproducible; el producto solicitado por UDG+ sigue siendo un enlace público al repositorio.

## Estado histórico al finalizar la actividad 4.4 — capítulos 7 y 8

El estado vigente de capítulos 9/10 se documenta al final; ahora sí hay avance real entre Stages y menú.

### Generación y mejoras visuales
- `HelixController.LoadStage(int)` valida referencias, stages y secciones; limita el índice al rango válido y restaura la rotación inicial de la torre.
- Destruye las plataformas de la carga anterior, limpia la lista e instancia las nuevas entre la plataforma superior y la meta. Las anclas permanentes nunca se destruyen. En Play, las instancias salientes se desactivan y separan antes de su destrucción diferida.
- La cámara utiliza Solid Color y `Stage.backgroundColor`; la bola recibe `ballColor`. Las piezas seguras reciben `helixColor` y DeathPart conserva rojo. Se usa MaterialPropertyBlock compatible con URP para no alterar materiales originales compartidos.
- Solo los sectores con MeshCollider participan en la selección: el objeto hijo del trigger de puntuación nunca se desactiva como si fuera una pieza.
- Los huecos y peligros se seleccionan aleatoriamente sin repetición. Cantidades fuera de rango se limitan para evitar bucles imposibles; siempre queda al menos un sector abierto.
- La generación ocurre al iniciar Play o llamar LoadStage. Perder conserva el diseño del intento y reinicia bola/Score/triggers; no regenera automáticamente todo el Stage ni cambia de nivel.

### Configuración de ejemplo jugable
`Assets/HelixJump/Stages/Stage_Christmas_78.asset` es el Stage conectado a la escena. Contiene tres anillos intermedios: **8 sólidos/1 peligro**, **9/2** y **10/2**. Se usan tres secciones para respetar el espacio de la torre existente; el antiguo Stage_Example con 12 secciones se conserva como recurso didáctico y no está conectado.

Cada anillo tiene 12 sectores antes de abrir huecos. Las alturas locales verificadas son 1.475, -0.05 y -1.575; distancia entre anclas con margen: 6.1. La plataforma superior fija conserva su hueco y peligro. Conservamos bola verde, seguras marfil, peligros rojos, fondo menta, torre verde pino, meta dorada y HUD legible.

Edita los colores y la lista levels del Stage conectado desde el Inspector **fuera de Play** y vuelve a iniciar para ver los cambios. No se añaden luces extra, posprocesado, menús, dificultad automática ni transición real al siguiente Stage. La meta mantiene el aviso Pasamos de nivel del tutorial.

### Archivos añadidos y preparación
- `Prefabs/HelixLevel_Chapter78.prefab`: 12 sectores disponibles, sin peligros prefijados y con trigger intacto.
- `Editor/Chapter78Setup.cs`: migración ya aplicada; elimina la plataforma intermedia manual y conecta el Stage/prefab nuevos. No es necesario ejecutarlo para jugar; reabriría y reconfiguraría la escena guardada.
- Pruebas nuevas: `HelixJump44Tests.cs` y `HelixJump44SafetyTests.cs`. Las pruebas anteriores se adaptan a que solo el ancla superior está preconstruida.

### Verificación actual
Suite Unity Pipeline filtro HelixJump: **17/17 aprobadas**, cero fallos, omitidas o inconclusas. Incluye generación, posiciones, cantidades, trigger conservado, restauración de rotación, recarga sin duplicados, colores y datos vacíos/nulos/fuera de rango, más las regresiones de capítulos anteriores.

La prueba física de puntuación y muerte entra realmente en Play Mode con objetos aislados. Su espera se basa en tiempo físico, no en una cantidad arbitraria de frames del runner. Preserva la clave de PlayerPrefs mediante SessionState entre recargas de dominio.

Comprobación adicional en Play de los tres anillos generados: 12 sectores cada uno; activos 8/9/10 y peligros 1/2/2; colores de cámara, bola, seguros y peligros verificados. Se llamó LoadStage con índice alto y se comprobó que no duplica plataformas y restaura rotación. Se observó avance real de Time.time; captura de Game View 1080x1920 revisada visualmente.

Escena guardada fuera de Play; Run In Background vuelve al valor previo desactivado. No se genera APK ni se afirma validación Android físico. Git/GitHub y el envío existente en UDG+ no se modifican.

Apoyos consultados mediante transcripciones completas:
7. Lógica Levels 2: https://youtu.be/fVWx40nGfmc
8. Completar nivel: https://youtu.be/c0qAMPNQvYY

## Actividad 4.5 — capítulos 9 y 10 (estado vigente)

### Menú y navegación
- Abrir `Scenes/MainMenu.unity`: título Helix Jump, botones Jugar/Salir, Canvas adaptable 1080x1920, EventSystem con InputSystemUIInputModule y callbacks persistentes.
- MenuController.Play usa SceneManager.LoadScene para abrir HelixJump. MainMenu figura primero en escenas de compilación y HelixJump después; otras escenas ya existentes se conservan.
- Salir usa Application.Quit en la compilación. En el editor solo registra el mensaje de comprobación y no cierra Unity. No se afirma salida verificada en un ejecutable, pues no se generó build.

### Cambio real de Stage
- GameManager.NextLevel avanza dentro de la misma escena, restablece la bola y carga el Stage siguiente **sin borrar Score**. La meta se bloquea brevemente para evitar varios avances por contactos del mismo paso físico.
- RestartLevel pone Score a cero, conserva Best y recarga el Stage actual con otra distribución aleatoria. TrailRenderer se limpia al restituir la bola para evitar una línea de teletransporte.
- Se conectaron tres assets propios `Stages/Stage_Christmas_910_01.asset`, `_02.asset` y `_03.asset`: tres anillos por Stage, paletas menta/hielo/cálida con verdes de bola, seguras claras y peligros rojos.
- Al agotar la lista se limita el índice al último Stage y se recarga este. No se añade pantalla de victoria, regreso automático al menú, pausa, ajustes, vidas ni UI de récord en menú.

### Ajustes visuales
- Se retiró DeathPart de prueba de la plataforma superior fija.
- Cámara con X original y pequeños giros Y=-0.5 y Z=8 grados.
- Dos luces direccionales cálida/fría cercana al blanco, intensidades 0.5 y 1, ambas sin sombras.
- Se conserva URP y su renderer Forward existente (Mobile_Renderer). No se copia la opción Deferred de la cámara Built-in del tutorial, ni se cambia el pipeline global. La cámara permite MSAA, pero el asset actual tiene una muestra; no se afirma antialiasing 4x activado.
- Impulso final **4**: se toma 5.5 del vídeo como referencia, pero se ajusta al tamaño y separación actuales para conservar margen bajo las plataformas. Masa 1, esfera de radio mundial 0.15 y separación 1.525. No se ampliaron arbitrariamente la torre ni las anclas.
- BallTrail: TrailRenderer con material URP Particles/Unlit transparente, duración 0.25, ancho decreciente 0.18 a cero, distancia mínima 0.2 y gradiente fijo menta/verde con transparencia final. El trazo es deliberadamente sutil; se comprobó emitiendo puntos en Play y su limpieza en ResetBall.
- `Editor/Chapter910Setup.cs` ya aplicado. No ejecutar setups anteriores para continuar. El setup vuelve a crear el menú, por lo que no conviene ejecutarlo sobre ajustes manuales posteriores sin respaldo.

### Pruebas actuales
Suite HelixJump en Unity Pipeline: **20/20 aprobadas**, cero fallos, omitidas o inconclusas. Incluye las regresiones anteriores y nuevas comprobaciones de menú, callbacks, Input System, orden de escenas, luces y material de estela.

Una UnityTest entra a Play desde MainMenu, ejecuta los botones, carga la escena por SceneManager y verifica dos contactos físicos sucesivos con la meta: Stage 0 → 1 → 2. Para aislar las transiciones, sitúa la bola encima de la meta y deja que la física produzca la colisión; no se presenta como recorrido manual completo de cada Stage. Comprueba Score conservado, cambios de color, ausencia de plataformas duplicadas, reinicio en Stage correcto y conservación de Best. Otra prueba verifica contactos duplicados. Las pruebas restauran únicamente la clave específica de PlayerPrefs usando SessionState a través de los reloads.

Además, un ratón virtual temporal atravesó el InputSystemUIInputModule y el raycast del botón Jugar, comprobando el cambio real MainMenu → HelixJump; se retiró ese dispositivo al finalizar. No se afirma prueba táctil física. Capturas reales de menú y juego en 1080x1920 revisadas, sin solapamientos importantes.

Al finalizar, escenas guardadas, MainMenu abierto fuera de Play, compilación sin fallos y override Run In Background restaurado a false. Sin APK, ejecutable ni validación Android físico. No hubo operaciones Git/GitHub ni envío a UDG+.

Apoyos consultados mediante transcripciones completas:
9. Mejora Visual y cambio nivel: https://youtu.be/9t5BCUVNLCo
10. Main Menu - Trail: https://youtu.be/F0n-oubaVVU



