using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Kinect;
using EstudandoKinect3;
using AuxiliarKinect.FuncoesBasicas;

namespace Auxiliar
{
    static class Extensao
    {
        /*public static void DesenharEsqueletoUsuario(this SkeletonFrame quadro, KinectSensor kinectSensor, Canvas canvasParaDesenhar)
        {
            if (kinectSensor == null) throw new ArgumentNullException("kinectSensor");
            if (canvasParaDesenhar == null) throw new ArgumentNullException("canvasParaDesenhar");

            Skeleton esqueleto = ObterEsqueletoUsuario(quadro);

            if (esqueleto != null)
            {
                EsqueletoUsuarioAuxiliar esqueletoUsuarioAuxiliar = new EsqueletoUsuarioAuxiliar(kinectSensor);
                foreach (Joint articulacao in esqueleto.Joints) esqueletoUsuarioAuxiliar.DesenharArticulacao(articulacao, canvasParaDesenhar);
            }
        }*/

        public static void DesenharEsqueletoUsuario(this SkeletonFrame quadro, KinectSensor kinectSensor, Canvas canvasParaDesenhar)
        {
            if (kinectSensor == null) throw new ArgumentNullException("kinectSensor");
            if (canvasParaDesenhar == null) throw new ArgumentNullException("canvasParaDesenhar");
            Skeleton[] esqueletos = new Skeleton[quadro.SkeletonArrayLength];
            quadro.CopySkeletonDataTo(esqueletos);
            IEnumerable<Skeleton> esqueletosRastreados = esqueletos.Where(esqueleto => esqueleto.TrackingState == SkeletonTrackingState.Tracked);
            if (esqueletosRastreados.Count() > 0)
            {
                Skeleton esqueleto = esqueletosRastreados.First();
                EsqueletoUsuarioAuxiliar esqueletoUsuarioAuxiliar = new EsqueletoUsuarioAuxiliar(kinectSensor);

                foreach(BoneOrientation osso in esqueleto.BoneOrientations)
                {
                    //desenhar ligação entre os joints (linha do esqueleto)
                    esqueletoUsuarioAuxiliar.DesenharOsso(esqueleto.Joints[osso.StartJoint], esqueleto.Joints[osso.EndJoint], canvasParaDesenhar);
                        
                    esqueletoUsuarioAuxiliar.DesenharArticulacao(esqueleto.Joints[osso.EndJoint], canvasParaDesenhar);
                }

                //foreach (Joint articulacao in esqueleto.Joints) esqueletoUsuarioAuxiliar.DesenharArticulacao(articulacao, canvasParaDesenhar);
            }
        }

        public static Skeleton ObterEsqueletoUsuario(this SkeletonFrame quadro)
        {
            Skeleton esqueletoUsuario = null;

            Skeleton[] esqueletos = new Skeleton[quadro.SkeletonArrayLength];

            quadro.CopySkeletonDataTo(esqueletos);

            IEnumerable<Skeleton> esqueletosRastreados = esqueletos.Where(esqueleto => esqueleto.TrackingState == SkeletonTrackingState.Tracked);

            if (esqueletosRastreados.Count() > 0) esqueletoUsuario = esqueletosRastreados.First();

            return esqueletoUsuario;
        }
    }
}
