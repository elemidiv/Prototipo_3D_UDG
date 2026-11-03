# Helix Jump — Actividad 4.1

Implementación de los dos primeros capítulos de LuisCanary, adaptada a Unity **6000.6.7f1** y URP.

## Abrir y probar

1. Abre `Assets/HelixJump/Scenes/HelixJump.unity`.
2. En Game selecciona la resolución **Helix Jump Portrait (1080 × 1920)**, o cualquier proporción vertical 9:16.
3. Pulsa Play. La bola verde cae y rebota sobre la plataforma superior.
4. Para repetir la demostración del segundo tutorial, SIN salir de Play, abre:
   `Helix > HelixLevel_01` y desactiva `HelixPart_00` y `HelixPart_01`.
5. La bola cae por el hueco hasta la segunda plataforma; la cámara la acompaña solo en Y.
6. Detén Play para restaurar los cambios temporales.

**No hay controles táctiles ni rotación de torre**: esos mecanismos no se desarrollan en los dos capítulos solicitados. Tampoco se incorporaron puntuación, victoria/derrota, generación de niveles, menús o características de capítulos posteriores.

## Implementado

- Modelo `Assets/Models/HelixPart.FBX` y prefab originales importados por el estudiante.
- Prefab reutilizable `Assets/HelixJump/Prefabs/HelixLevel.prefab`, compuesto por sectores cada 30° y MeshColliders con su malla asignada.
- Dos plataformas, torre central y bola con SphereCollider/Rigidbody.
- Rebote mediante impulso de 3 N·s con masa de 1 kg, reinicio de velocidad y bloqueo de contactos duplicados durante 0.2 s.
- Bola restringida a Y, sin rotación. Detección continua e interpolación.
- Seguimiento de cámara en Y con separación inicial constante, manteniendo X/Z. Actualización en LateUpdate.
- Material físico sin rebote adicional para no acumular impulsos; materiales gráficos compatibles con URP.
- Canvas adaptable a resolución con un único texto: **Helix Jump**.
- Paleta navideña: bola verde esmeralda, plataformas rojo arándano/blanco cálido, torre verde pino, fondo menta y contorno dorado en el título.
- Orientación vertical y escena incluida en la lista de compilación. La plataforma Android ya estaba activa y sus módulos estaban disponibles.

## Archivos

- `Assets/HelixJump/Scripts/BallController.cs`: rebote y restricciones.
- `Assets/HelixJump/Scripts/CameraFollow.cs`: seguimiento vertical.
- `Assets/HelixJump/Scenes/HelixJump.unity`: escena principal.
- `Assets/HelixJump/Prefabs/`: plataforma y bola reutilizables.
- `Assets/HelixJump/Materials/` y `Physics/`: paleta URP y material físico.
- `Assets/HelixJump/Tests/Editor/`: pruebas de rebote, cámara y configuración de escena.
- `Assets/HelixJump/Editor/TutorialSceneBuilder.cs`: construcción inicial. No sobrescribe una escena existente; no es necesario ejecutarlo para jugar.

Los assets originales importados y SampleScene se conservaron. No se ejecutaron comandos Git ni operaciones GitHub.

## Validación realizada

- Tres pruebas EditMode aprobadas: rebote/antiduplicado/rearme; separación de cámara Y y X/Z fijos; componentes, prefabs, colliders, título y orientación de escena.
- Play Mode: rebotes repetidos sobre la unión de dos sectores.
- Play Mode: apertura temporal de las dos piezas bajo la bola y descenso hasta la plataforma inferior, con nuevo rebote y separación de cámara de 3 unidades.
- Captura real de Game View en 1080 × 1920, revisada visualmente.

No se generó APK ni se probó en un dispositivo Android físico. No se afirma validación de rendimiento móvil.

## Tutoriales

1. Crear tu primer Videojuego con Unity/HelixJump: https://youtu.be/QYUgb5pHi9c
2. Crea tu primer Videojuego para Android con Unity/HelixJump/Fisicas Bola/2-Cap: https://youtu.be/VHLz-UHErnA
