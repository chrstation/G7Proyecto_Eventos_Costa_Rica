EVENTOS COSTA RICA - VENTA DE ENTRADAS PARA EVENTOS
===================================================

1. INTEGRANTES FINALES DEL GRUPO
   - Herberth Masis Canales
   - Michael Castro Herrera
   - Carlos Villalobos Picado
   - Andy Segura

2. REPOSITORIO
   https://github.com/chrstation/G7Proyecto_Eventos_Costa_Rica.git

3. ESPECIFICACION BASICA DEL PROYECTO

   a. Arquitectura (estructura por capas, .NET 10, ASP.NET Core MVC)
      - EventosCostaRica.Entidades : Class Library. Modelos del dominio (Usuario, Evento,
                                     Entrada, Asiento, RolUsuario, EntidadBase).
      - EventosCostaRica.Datos     : Class Library. CAPA DE ACCESO A DATOS. DbContext de
                                     Entity Framework Core (SQLite) y repositorio generico
                                     (IRepositorio<T> / Repositorio<T>).
      - EventosCostaRica.Negocio   : Class Library. CAPA LOGICA DE NEGOCIO. Servicios de
                                     usuarios, eventos y entradas, con sus validaciones, y
                                     el hash de contrasenas.
      - EventosCostaRica.Web       : ASP.NET Core MVC (presentacion). Controladores,
                                     ViewModels, vistas Razor, autenticacion, inyeccion de
                                     dependencias y manejador global de excepciones.

      Dependencias: Web -> Negocio -> Datos -> Entidades

      Modulos:
      - Log In: autenticacion con cookies. Roles: Administrador y Cliente.
      - Administracion de usuarios (solo administradores): crear, listar, modificar y borrar.
      - Administracion de eventos (solo administradores): crear, listar, modificar y borrar.
      - Pagina de inicio (administradores y clientes): lista de proximos eventos.
      - Detalle de evento: fecha, lugar, banner y si esta vendido (agotado) o disponible.
      - Compra de boletos: mapa de 10 filas (A-J) de 10 asientos (100 por evento). Se pueden
        elegir hasta 10 asientos por compra. No incluye el pago.
      - Mis boletos: evento, fecha y asientos comprados por el usuario.

      Seguridad: toda la aplicacion exige iniciar sesion; las pantallas de administracion
      requieren el rol Administrador. Las contrasenas se guardan con hash (PBKDF2 con sal
      aleatoria), nunca en texto plano. Los formularios usan token antifalsificacion.

      Validaciones principales:
      - Correo de usuario unico y con formato valido; contrasena de al menos 6 caracteres.
      - No se puede borrar el ultimo administrador, ni a uno mismo, ni un usuario con
        entradas compradas.
      - La fecha de un evento nuevo debe ser futura; no se puede borrar un evento con
        entradas vendidas.
      - Un asiento no se puede vender dos veces: se valida en la logica de negocio y ademas
        hay un indice unico (evento, fila, numero) en la base de datos.
      - Los Id son autonumericos, por lo que no se repiten.

      Manejador global de excepciones: ManejadorGlobalExcepciones implementa
      IExceptionHandler; registra los errores no controlados y muestra la pagina /Error.
      Para probarlo: /Error/Probar.

      Base de datos: SQLite con Entity Framework Core. El archivo eventoscostarica.db se
      crea automaticamente al ejecutar (EnsureCreated), con datos de ejemplo.

      Usuarios de ejemplo:
        Administrador: admin@eventoscr.com   / Admin123!
        Cliente:       cliente@eventoscr.com / Cliente123!

   b. Paquetes NuGet y librerias
      - Microsoft.EntityFrameworkCore.Sqlite (10.0.x)
      - Bootstrap 5.3 (estilos, cargado desde CDN en _Layout.cshtml; requiere internet)

   c. Principios SOLID y patrones de diseno
      SOLID:
      - S (Responsabilidad unica): repositorio = datos, servicios = reglas de negocio,
        controladores = flujo HTTP, hasheador = contrasenas, manejador global = errores.
      - O (Abierto/cerrado): el repositorio generico sirve para nuevas entidades sin
        modificarlo.
      - L (Sustitucion de Liskov): cualquier implementacion de IRepositorio<T>, de las
        interfaces de servicio o de IHasheadorContrasena puede sustituir a otra.
      - I (Segregacion de interfaces): interfaces pequenas y especificas.
      - D (Inversion de dependencias): controladores y servicios dependen de
        abstracciones (interfaces), no de clases concretas.

      Patrones:
      - MVC (Model - View - Controller).
      - Repository generico (Repositorio<T>).
      - Service Layer (UsuarioServicio, EventoServicio, EntradaServicio).
      - Dependency Injection / Inversion of Control (Program.cs).
      - ViewModel / DTO.
      - Manejador global de excepciones (IExceptionHandler).
      - Arquitectura en capas (Layered Architecture).

EJECUCION
   Abrir EventosCostaRica.sln en Visual Studio 2026, establecer EventosCostaRica.Web
   como proyecto de inicio y presionar F5 (abre http://localhost:5110).
