using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Drawing;
using System.IO;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace EarthSystem
{
    class Sputnik
    {
        private Vector3 realPosition;

        private readonly float[] sputnik;
        private readonly float[] camera;
        private readonly float[] xaxis;
        private readonly float[] yaxis;

        private Shader sputnikShader;
        private Shader cameraShader;
        private Shader xaxisShader;
        private Shader yaxisShader;

        private int vaoSputnik;
        private int vaoCamera;
        private int vaoXaxis;
        private int vaoYaxis;

        private int sputnikBufferObject;
        private int cameraBufferObject;
        private int xaxisBufferObject;
        private int yaxisBufferObject;

        private float sputnikLenght = 200;
        private float sputnikWidth = 200;
        private float sputnikHeight = 200;

        private float limitAngle = 45;//предельный угол зрения камеры спутника

        public Sputnik(Vector3 realPosition)
        {
            sputnik = new float[]
            {   
                //-carWidth/2 -> 0, carWidth/2 -> carWidth чтобы центр машинки был на плоскости дна
                //кубик без одной грани - спутник
                //sputnikLenght/2,-sputnikWidth/2,-sputnikHeight/2, sputnikLenght/2,sputnikWidth/2,-sputnikHeight/2, sputnikLenght/2,-sputnikWidth/2,sputnikHeight/2, sputnikLenght/2,sputnikWidth/2,sputnikHeight/2, sputnikLenght/2,sputnikWidth/2,-sputnikHeight/2, sputnikLenght/2,-sputnikWidth/2,sputnikHeight/2,//front
                -sputnikLenght/2,-sputnikWidth/2,-sputnikHeight/2, -sputnikLenght/2,sputnikWidth/2,-sputnikHeight/2, -sputnikLenght/2,-sputnikWidth/2,sputnikHeight/2, -sputnikLenght/2,sputnikWidth/2,sputnikHeight/2, -sputnikLenght/2,sputnikWidth/2,-sputnikHeight/2, -sputnikLenght/2,-sputnikWidth/2,sputnikHeight/2,//back
                //sputnikLenght/2,-sputnikWidth/2,sputnikHeight/2, sputnikLenght/2,sputnikWidth/2,sputnikHeight/2, -sputnikLenght/2,-sputnikWidth/2,sputnikHeight/2, -sputnikLenght/2,sputnikWidth/2,sputnikHeight/2, sputnikLenght/2,sputnikWidth/2,sputnikHeight/2, -sputnikLenght/2,-sputnikWidth/2,sputnikHeight/2,//up
                sputnikLenght/2,-sputnikWidth/2,-sputnikHeight/2, sputnikLenght/2,sputnikWidth/2,-sputnikHeight/2, -sputnikLenght/2,-sputnikWidth/2,-sputnikHeight/2, -sputnikLenght/2,sputnikWidth/2,-sputnikHeight/2, sputnikLenght/2,sputnikWidth/2,-sputnikHeight/2, -sputnikLenght/2,-sputnikWidth/2,-sputnikHeight/2,//down
                sputnikLenght/2,-sputnikWidth/2,-sputnikHeight/2, -sputnikLenght/2,-sputnikWidth/2,-sputnikHeight/2, sputnikLenght/2,-sputnikWidth/2,sputnikHeight/2, -sputnikLenght/2,-sputnikWidth/2,sputnikHeight/2, -sputnikLenght/2,-sputnikWidth/2,-sputnikHeight/2, sputnikLenght/2,-sputnikWidth/2,sputnikHeight/2,//left
                //sputnikLenght/2,sputnikWidth/2,-sputnikHeight/2, -sputnikLenght/2,sputnikWidth/2,-sputnikHeight/2, sputnikLenght/2,sputnikWidth/2,sputnikHeight/2, -sputnikLenght/2,sputnikWidth/2,sputnikHeight/2, -sputnikLenght/2,sputnikWidth/2,-sputnikHeight/2, sputnikLenght/2,sputnikWidth/2,sputnikHeight/2,//right
            };

            xaxis = new float[]
            {
                //пирамидка которая ознает ось x
                sputnikLenght/2,-sputnikWidth/2,-sputnikHeight/2,sputnikLenght/2,sputnikWidth/2,-sputnikHeight/2,sputnikLenght,0,0,
                sputnikLenght/2,sputnikWidth/2,-sputnikHeight/2,sputnikLenght/2,sputnikWidth/2,sputnikHeight/2,sputnikLenght,0,0,
                sputnikLenght/2,sputnikWidth/2,sputnikHeight/2,sputnikLenght/2,-sputnikWidth/2,sputnikHeight/2,sputnikLenght,0,0,
                sputnikLenght/2,-sputnikWidth/2,sputnikHeight/2,sputnikLenght/2,-sputnikWidth/2,-sputnikHeight/2,sputnikLenght,0,0,
            };

            yaxis = new float[]
            {
                //пирамидка которая ознает ось y
                -sputnikLenght/2,-sputnikWidth/2,sputnikHeight/2,sputnikLenght/2,-sputnikWidth/2,sputnikHeight/2,0,0,sputnikHeight,
                sputnikLenght/2,-sputnikWidth/2,sputnikHeight/2,sputnikLenght/2,sputnikWidth/2,sputnikHeight/2,0,0,sputnikHeight,
                sputnikLenght/2,sputnikWidth/2,sputnikHeight/2,-sputnikLenght/2,sputnikWidth/2,sputnikHeight/2,0,0,sputnikHeight,
                -sputnikLenght/2,sputnikWidth/2,sputnikHeight/2,-sputnikLenght/2,-sputnikWidth/2,sputnikHeight/2,0,0,sputnikHeight,
            };

            camera = new float[]
            {
                //пирамидка без дна - камера спутника а ещё выполняется роль оси z
                -sputnikLenght/2,sputnikWidth/2,-sputnikHeight/2,sputnikLenght/2,sputnikWidth/2,-sputnikHeight/2,0,sputnikWidth,0,
                sputnikLenght/2,sputnikWidth/2,-sputnikHeight/2,sputnikLenght/2,sputnikWidth/2,sputnikHeight/2,0,sputnikWidth,0,
                sputnikLenght/2,sputnikWidth/2,sputnikHeight/2,-sputnikLenght/2,sputnikWidth/2,sputnikHeight/2,0,sputnikWidth,0,
                -sputnikLenght/2,sputnikWidth/2,sputnikHeight/2,-sputnikLenght/2,sputnikWidth/2,-sputnikHeight/2,0,sputnikWidth,0,
            };
            //_vertexBufferObject = GL.GenBuffer();
            sputnikShader = new Shader("./Shaders/shader.vert", "./Shaders/shader.frag");
            cameraShader = new Shader("./Shaders/shader.vert", "./Shaders/shader.frag");
            xaxisShader = new Shader("./Shaders/shader.vert", "./Shaders/shader.frag");
            yaxisShader = new Shader("./Shaders/shader.vert", "./Shaders/shader.frag");

            this.realPosition = realPosition;
        }

        public void load()
        {
            //GL.Enable(EnableCap.DepthTest);
            //GL.DepthFunc(DepthFunction.Less);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            sputnikShader.Use();
            GL.GenVertexArrays(1, out vaoSputnik);
            GL.BindVertexArray(vaoSputnik);
            GL.GenBuffers(1, out sputnikBufferObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, sputnikBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, sputnik.Length * sizeof(float), sputnik, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            cameraShader.Use();
            GL.GenVertexArrays(1, out vaoCamera);
            GL.BindVertexArray(vaoCamera);
            GL.GenBuffers(1, out cameraBufferObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, cameraBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, camera.Length * sizeof(float), camera, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            xaxisShader.Use();
            GL.GenVertexArrays(1, out vaoXaxis);
            GL.BindVertexArray(vaoXaxis);
            GL.GenBuffers(1, out xaxisBufferObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, xaxisBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, xaxis.Length * sizeof(float), xaxis, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            yaxisShader.Use();
            GL.GenVertexArrays(1, out vaoYaxis);
            GL.BindVertexArray(vaoYaxis);
            GL.GenBuffers(1, out yaxisBufferObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, yaxisBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, yaxis.Length * sizeof(float), yaxis, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        }

        public void render(FrameEventArgs e, Matrix4 model)
        {
            //_frameshader.SetMatrix4("model", model);

            xaxisShader.Use();
            xaxisShader.SetMatrix4("view", Program.camera.GetViewMatrix());
            xaxisShader.SetMatrix4("projection", Program.camera.GetProjectionMatrix());
            xaxisShader.SetVector3("objectColor", new Vector3(1f, 0f, 0f));
            xaxisShader.SetMatrix4("model", model);
            GL.BindVertexArray(vaoXaxis);
            GL.DrawArrays(PrimitiveType.Triangles, 0, xaxis.Length);

            yaxisShader.Use();
            yaxisShader.SetMatrix4("view", Program.camera.GetViewMatrix());
            yaxisShader.SetMatrix4("projection", Program.camera.GetProjectionMatrix());
            yaxisShader.SetVector3("objectColor", new Vector3(0f, 1f, 0f));
            yaxisShader.SetMatrix4("model", model);
            GL.BindVertexArray(vaoYaxis);
            GL.DrawArrays(PrimitiveType.Triangles, 0, yaxis.Length);

            cameraShader.Use();
            cameraShader.SetMatrix4("view", Program.camera.GetViewMatrix());
            cameraShader.SetMatrix4("projection", Program.camera.GetProjectionMatrix());
            cameraShader.SetVector3("objectColor", new Vector3(0f, 0f, 1f));
            cameraShader.SetMatrix4("model", model);
            GL.BindVertexArray(vaoCamera);
            GL.DrawArrays(PrimitiveType.Triangles, 0, camera.Length);

            sputnikShader.Use();
            sputnikShader.SetMatrix4("view", Program.camera.GetViewMatrix());
            sputnikShader.SetMatrix4("projection", Program.camera.GetProjectionMatrix());
            sputnikShader.SetVector3("objectColor", new Vector3(1f, 1f, 1f));
            sputnikShader.SetMatrix4("model", model);
            GL.BindVertexArray(vaoSputnik);
            GL.DrawArrays(PrimitiveType.Triangles, 0, sputnik.Length);
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);//закрашивать или оставить каркас
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

        }

        public void destroy(EventArgs e)
        {
            GL.DeleteProgram(sputnikShader.Handle);
            GL.DeleteProgram(cameraShader.Handle);
            GL.DeleteProgram(xaxisShader.Handle);
            GL.DeleteProgram(yaxisShader.Handle);
        }

        public Vector3 returnCameraPos()
        {
            return new Vector3(0, sputnikWidth, 0);
        }

        public Vector3 returnRealPos()
        {
            return realPosition;
        }

        public void assignRealPosition(Vector3 realPosition)
        {
            this.realPosition = realPosition;
        }

        public int numberVisibleSputnik(List<Vector3> Sputniks,Vector3 SPos,Vector3 CamPos)
        {
            int result = 0;
            for(int i=0;i<Sputniks.Count;i++)
            {
                if(Sputniks[i] != SPos && MathHelper.RadiansToDegrees(Math.Acos(Vector3.Dot(CamPos - SPos, Sputniks[i] - CamPos) / ((CamPos - SPos).Length * (Sputniks[i]-CamPos).Length)))<=limitAngle)
                {
                    result++;
                }
            }
            return result;
        }

    }
}