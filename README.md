# SoulAir: Experiencia Interactiva sobre la Calidad del Aire
SoulAir es un sistema multimedia interactivo desarrollado en **Unity** como un proyecto académico para la materia de Diseño Multimedia 2 en la Universidad Autónoma de Occidente.
El objetivo principal del proyecto es informar y sensibilizar a los usuarios sobre los niveles de calidad del aire en su entorno, específicamente en la ciudad de Cali, y educar sobre las medidas de protección contra materiales particulados como el PM 2.5 y PM 10

### Características Principales

* **Mapa Interactivo:** Visualización en tiempo real de la calidad del aire (ICA) y focos de congestión vehicular. El mapa se renderiza utilizando **Mapbox SDK** y se alimenta de datos de la **HERE Traffic API** para el flujo de tráfico de **Firebase Firestore** para los niveles de ICA.
* **Avatar Reactivo:** Un personaje central en forma de nube, diseñado en 2D y 3D, cuya expresión y estado cambian dinámicamente según los niveles de contaminación reportados. Sirve como un indicador emocional y visual para el usuario.
* **Minijuego Educativo:** Un juego estilo *runner* en 3D donde el usuario controla al avatar en una bicicleta, esquivando obstáculos contaminantes y aprendiendo sobre hábitos saludables. El personaje fue modelado en **Blender** y animado con **Mixamo**.
* **Herramienta de Reporte Ciudadano:** Funcionalidad que permite a los usuarios reportar incidentes que afectan la calidad del aire (incendios, construcciones, tráfico denso, etc.). Los reportes son almacenados de forma segura en **Firebase Firestore**, con un sistema de cifrado para proteger la información del usuario.
* **Realidad Aumentada (AR):** Módulo que permite al usuario visualizar una representación del material particulado en su entorno cercano a través de la cámara del dispositivo.

### Tecnologías Utilizadas

**Motor de Desarrollo:** Unity 
**Backend y Base de Datos:** Firebase (Authentication, Firestore)
**APIs Externas:** Mapbox Maps SDK [cite: 528][cite_start], HERE Traffic API 
**Herramientas de Diseño:** Inkscape, Blender, Mixamo, Adobe Illustrator, Figma.
**Control de Versiones:** GitHub, Git LFS 



## 1. ✅ Requisitos Iniciales

- Dispositivo Android con versión mínima **8.0**.
- Permitir instalación desde **fuentes desconocidas**:
  - Ve a `Ajustes > Seguridad > Fuentes desconocidas` y habilita la opción.
- Descargar el archivo `.apk` desde este repositorio. *(Enlace por confirmar)*

---

## 2. 🔐 Autenticación de Usuario

Al iniciar la aplicación, verás una pantalla de autenticación con dos opciones:

### 🆕 Registrarse (Usuarios nuevos)
1. Presiona el botón **“Registrarse”**.
2. Completa los siguientes campos:
   - Nombre de usuario
   - Correo electrónico
   - Contraseña segura
3. Presiona **“Enviar”** para crear tu cuenta.

### 🔓 Iniciar Sesión (Usuarios registrados)
1. Ingresa el **correo electrónico** y la **contraseña registrada**.
2. Presiona **“Iniciar Sesión”** para acceder a la aplicación.

> ⚠️ **Importante:** Es indispensable registrarse antes de iniciar sesión si es la primera vez que usas el demo.

---

## 3. 🗺️ Mapa Interactivo – Instrucciones de Uso

Una vez autenticado, ingresarás al **mapa interactivo**, desarrollado con Unity, que muestra información en tiempo real sobre la **calidad del aire** en comunas seleccionadas.

### 🧭 Funcionamiento del Mapa

- El mapa presenta un **gráfico de densidad** que destaca la calidad del aire en las comunas **17, 18 y 22**.
- Se utilizan los colores del **Índice de Calidad del Aire (ICA)** para representar los niveles de contaminación:

  - 🟢 **Buena:** 0–50  
  - 🟡 **Moderada:** 51–100  
  - 🟠 **Dañina para grupos sensibles:** 101–150  
  - 🔴 **Dañina a la salud:** 151–200

- **Índice numérico (ICA):**  
  El número en el centro de la pantalla indica el **valor promedio actual** del ICA en tu ubicación o comuna seleccionada.

- **Origen de los datos:**  
  Los valores provienen de sistemas de vigilancia ambiental como **DAGMA**, **Grupo Tángara** y servicios abiertos como `api.waqi.info`.

- **Permiso de ubicación:**  
  La aplicación solicitará acceso a tu ubicación en tiempo real. Selecciona **“Aceptar”** para visualizar los datos locales.

---

## 4. 🌍 Realidad Aumentada (RA) – Funcionalidad Básica

Presiona el botón **“RA”** en la parte inferior izquierda para acceder al modo de realidad aumentada.

### ¿Qué verás en este modo?
- Una **superposición visual** sobre el entorno real capturado por la cámara.
- Partículas contaminantes **PM2.5**.
- Un **video ilustrativo** sobre cómo el PM2.5 afecta a tu cuerpo.

---

## 📝 Notas Finales

- Esta demo representa el **primer sprint**, centrado en prototipar las funcionalidades básicas del **mapa interactivo** y la **realidad aumentada**.
- Para **reportar errores** o enviar sugerencias, utiliza el canal oficial del proyecto en GitHub.

---

Proyecto para la materia DM2 - Ingeniería Multimedia IM06 - Universidad Autónoma del Occidente 
