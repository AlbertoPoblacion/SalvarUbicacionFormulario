# SalvarUbicacionFormulario
Salva y recupera la ubicacion en el escritorio de formularios Winforms

Contiene un proyecto de prueba con un simple formulario "Form1". En el evento Form_Closing se salva su ubicación, y en el Form_Load se recupera.

Soporta núltiples monitores con distintas resoluciones, y soporta abrir la aplicación desde escritorio remoto con distintas resoluciones, salvando una configuración distinta por cada resolución.

Es importante leer el comentario que hay dentro de la clase bajo "Herramientas", ya que ahí describe la entrada que es necesario crear manualmente dentro de Properties.Settings en el proyecto, para que ahí se salven los datos.

