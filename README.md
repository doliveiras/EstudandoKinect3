# EstudandoKinect3

#Descrição
Projeto responsável por reconhecimento dos movimentos do usuário e extração de dados.

##Pré-Condição
Necessário add projeto AuxiliarKinect

## Responsáveis
* Daniel Oliveira
* Natalia Mazzoni
* Lucas Lozano
 
##Organização da pasta
```
└───EstudandoKinect3
    ├───bin
    │   ├───Debug
    │   └───Release
    ├───obj
    │   ├───Debug
    │   │   └───TempPE
    │   └───Release
    │       └───TempPE
    └───Properties
```

##Modificações
###01/06/2016
#####Desenvolvedores
* Daniel Oliveira

#####Descrição
Realização da primeira inclusão do projeto EstudandoKinect3

####**Pasta**/*Classe*/Métodos####
* **/**
	* *Extensao*
		* DesenharEsqueletoUsuario
		  * @param this.SkeletonFrame, KinectSensor, Canvas
		  * @return void
	
  * *EsqueletoUsuarioAuxiliar*
    * EsqueletoUsuarioAuxiliar
      * @param KinectSensor
    * ConverterCoordenadasArticulacao
    	* @param Joing, double, double
    	* @return ColorImagePoint
    * CriarComponenteVisualArticulacao
    	* @param int, int, Brush
      	* @return ColorImagePoint
    * DesenharArticulacao
      	* @param Joint, Canvas
    * CriarComponenteVisualOsso
      	* @param int, Brush, double, double, double, double 
      	* @return Line
    * DesenharOsso
      	* @param Joint, Joint, Canvas

  * *MainWindow.xaml.cs*
    * MainWindow
    * InicializarKinect
      	* @param KinectSensor
    * ReconhecerHumanos
      	* @param DepthImageFrame
      	* @return BitmapSource
    * ReconhecerDistancia
      	* @param DepthImageFrame, byte[], int
    * slider_DragCompleted
      	* @param object, System.Windows.Controls.Primitives.DragCompletedEventArgs
    * kinect_AllFramesReady
      	* @param object, AllFramesReadyEventArgs
    * DesenharEsqueletoUsuario
      	* @param SkeletonFrame
    * ObterImagemSensorRGB
      	* @param ColorImageFrame
      	* @return byte[]
