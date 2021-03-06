﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using AuxiliarKinect.FuncoesBasicas;
using Auxiliar;

namespace EstudandoKinect3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor kinect;
        public MainWindow()
        {
            InitializeComponent();
            InicializadorSeletor();
        }

        private void InicializadorSeletor()
        {
            InicializadorKinect inicializador = new InicializadorKinect();
            InicializarKinect(inicializador.SeletorKinect.Kinect);
            seletorSensorUI.KinectSensorChooser = inicializador.SeletorKinect;
        }

        private void InicializarKinect(KinectSensor kinectSensor)
        {
            //Recebe informações do sensor kinect
            kinect = kinectSensor;

            //Slider com eixo motor do kinect
            slider.Value = kinect.ElevationAngle;

            kinect.DepthStream.Enable();
            kinect.SkeletonStream.Enable();
            kinect.ColorStream.Enable();
            kinect.AllFramesReady += kinect_AllFramesReady;

            //kinect.DepthStream.Range = DepthRange.Near; //Versão Kinect for Windows

            //Pegar Apenas Imagem sem profundidade
            //kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            //kinect.ColorFrameReady += kinect_ColorFrameReady;
        }

        //Reconhecer pessoas com profundidade
        private BitmapSource ReconhecerHumanos(DepthImageFrame quadro)
        {
            if (quadro == null) return null;

            using (quadro)
            {
                DepthImagePixel[] imagemProfundidade = new DepthImagePixel[quadro.PixelDataLength];
                quadro.CopyDepthImagePixelDataTo(imagemProfundidade);
                byte[] bytesImagem = new byte[imagemProfundidade.Length * 4];
                for (int indice = 0; indice < bytesImagem.Length; indice += 4)
                {
                    if (imagemProfundidade[indice / 4].PlayerIndex != 0)
                    {
                        bytesImagem[indice + 1] = 255;
                    }
                }
                return BitmapSource.Create(quadro.Width, quadro.Height, 96, 96, PixelFormats.Bgr32, null, bytesImagem, quadro.Width * 4);
            }
        }

        //old Métodos para imagens sem profundidade
        /*
        private void kinect_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            imagemCamera.Source = ReconhecerHumanos(e.OpenDepthImageFrame());
        }

        private byte[] ObterImagemSensorRGB(ColorImageFrame quadro)
        {
            if (quadro == null) return null;
            using (quadro)
            {
                byte[] bytesImagem = new byte[quadro.PixelDataLength];
                quadro.CopyPixelDataTo(bytesImagem);
                return bytesImagem;
            }
        }*/

        //Reconhece a distancia
        private void ReconhecerDistancia(DepthImageFrame quadro, byte[] bytesImagem, int distanciaMaxima)
        {
            if (quadro == null || bytesImagem == null) return;
            using (quadro)
            {
                DepthImagePixel[] imagemProfundidade = new DepthImagePixel[quadro.PixelDataLength];
                quadro.CopyDepthImagePixelDataTo(imagemProfundidade);
                DepthImagePoint[] pontosImagemProfundidade = new DepthImagePoint[640 * 480];
                kinect.CoordinateMapper.MapColorFrameToDepthFrame(kinect.ColorStream.Format, kinect.DepthStream.Format, imagemProfundidade, pontosImagemProfundidade);
                for (int i = 0; i < pontosImagemProfundidade.Length; i++)
                {
                    var point = pontosImagemProfundidade[i]; if (point.Depth < distanciaMaxima && KinectSensor.IsKnownPoint(point))
                    {
                        var pixelDataIndex = i * 4;
                        byte maiorValorCor = Math.Max(bytesImagem[pixelDataIndex], Math.Max(bytesImagem[pixelDataIndex + 1], bytesImagem[pixelDataIndex + 2]));
                        bytesImagem[pixelDataIndex] = maiorValorCor; bytesImagem[pixelDataIndex + 1] = maiorValorCor; bytesImagem[pixelDataIndex + 2] = maiorValorCor;
                    }
                }

            }
        }

        //old
        /*private void kinect_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            byte[] imagem = ObterImagemSensorRGB(e.OpenColorImageFrame());
            if (chkEscalaCinza.IsChecked.HasValue && chkEscalaCinza.IsChecked.Value) ReconhecerDistancia(e.OpenDepthImageFrame(), imagem, 2000);
            if (imagem != null) imagemCamera.Source = BitmapSource.Create(kinect.ColorStream.FrameWidth, kinect.ColorStream.FrameHeight, 96, 96, PixelFormats.Bgr32, null, imagem, kinect.ColorStream.FrameBytesPerPixel * kinect.ColorStream.FrameWidth);

        }*/

        //método para atrelar o slider junto ao slider
        private void slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            kinect.ElevationAngle = Convert.ToInt32(slider.Value);
        }

        //Reconhece o usuario e se estiver habilidade a escala em cinza o seleciona nesta cor
        private void kinect_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            byte[] imagem = ObterImagemSensorRGB(e.OpenColorImageFrame());
            if (chkEscalaCinza.IsChecked.HasValue && chkEscalaCinza.IsChecked.Value) ReconhecerDistancia(e.OpenDepthImageFrame(), imagem, 2000);
            if (imagem != null) canvasKinect.Background = new ImageBrush(BitmapSource.Create(kinect.ColorStream.FrameWidth, kinect.ColorStream.FrameHeight, 96, 96, PixelFormats.Bgr32, null, imagem, kinect.ColorStream.FrameBytesPerPixel * kinect.ColorStream.FrameWidth));
            canvasKinect.Children.Clear();

            if( chkEsqueleto.IsChecked.HasValue && chkEsqueleto.IsChecked.Value)
            {
                DesenharEsqueletoUsuario(e.OpenSkeletonFrame());
            }
        }

        //Responsavel por identificar a mão direita e esquerda
        private void DesenharEsqueletoUsuario(SkeletonFrame quadro)
        {
            if (quadro == null) return;
            
            using (quadro) quadro.DesenharEsqueletoUsuario(kinect, canvasKinect);
           
            //old sem Extensao
            /*using (quadro)
            {
            
                Skeleton[] esqueletos = new Skeleton[quadro.SkeletonArrayLength];
                quadro.CopySkeletonDataTo(esqueletos);
                IEnumerable<Skeleton> esqueletosRastreados = esqueletos.Where(esqueleto => esqueleto.TrackingState == SkeletonTrackingState.Tracked);
                if (esqueletosRastreados.Count() > 0)
                {
                    Skeleton esqueleto = esqueletosRastreados.First();
                    EsqueletoUsuarioAuxiliar funcoesEsqueletos = new EsqueletoUsuarioAuxiliar(kinect);
                    funcoesEsqueletos.DesenharArticulacao(esqueleto.Joints[JointType.HandRight], canvasKinect);
                    funcoesEsqueletos.DesenharArticulacao(esqueleto.Joints[JointType.HandLeft], canvasKinect);
                }
            }*/
        }
    
        //Obtem imagem do kinect e o retorna
        private byte[] ObterImagemSensorRGB(ColorImageFrame quadro)
        {
            if (quadro == null) return null;
            using (quadro)
            {
                byte[] bytesImagem = new byte[quadro.PixelDataLength];
                quadro.CopyPixelDataTo(bytesImagem);
                return bytesImagem;
            }
        }

        /*private void Inicio (Object sender)
        {
            InicializadorSeletor();
        }*/
    }
}