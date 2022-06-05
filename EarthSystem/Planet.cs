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
    class Planet
    {
        private readonly float[] planet;
        private Shader planetShader;
        private int vaoPlanet;
        private int planetBufferObject;

        private float planetLenght = 5000;
        private float planetWidth = 5000;
        private float planetHeight = 5000;

        public Planet()
        {
            planet = new float[]
            {   
                //-carWidth/2 -> 0, carWidth/2 -> carWidth чтобы центр машинки был на плоскости дна
                planetLenght/2,-planetWidth/2,-planetHeight/2, planetLenght/2,planetWidth/2,-planetHeight/2, planetLenght/2,-planetWidth/2,planetHeight/2, planetLenght/2,planetWidth/2,planetHeight/2, planetLenght/2,planetWidth/2,-planetHeight/2, planetLenght/2,-planetWidth/2,planetHeight/2,//front
                -planetLenght/2,-planetWidth/2,-planetHeight/2, -planetLenght/2,planetWidth/2,-planetHeight/2, -planetLenght/2,-planetWidth/2,planetHeight/2, -planetLenght/2,planetWidth/2,planetHeight/2, -planetLenght/2,planetWidth/2,-planetHeight/2, -planetLenght/2,-planetWidth/2,planetHeight/2,//back
                planetLenght/2,-planetWidth/2,planetHeight/2, planetLenght/2,planetWidth/2,planetHeight/2, -planetLenght/2,-planetWidth/2,planetHeight/2, -planetLenght/2,planetWidth/2,planetHeight/2, planetLenght/2,planetWidth/2,planetHeight/2, -planetLenght/2,-planetWidth/2,planetHeight/2,//up
                planetLenght/2,-planetWidth/2,-planetHeight/2, planetLenght/2,planetWidth/2,-planetHeight/2, -planetLenght/2,-planetWidth/2,-planetHeight/2, -planetLenght/2,planetWidth/2,-planetHeight/2, planetLenght/2,planetWidth/2,-planetHeight/2, -planetLenght/2,-planetWidth/2,-planetHeight/2,//down
                planetLenght/2,-planetWidth/2,-planetHeight/2, -planetLenght/2,-planetWidth/2,-planetHeight/2, planetLenght/2,-planetWidth/2,planetHeight/2, -planetLenght/2,-planetWidth/2,planetHeight/2, -planetLenght/2,-planetWidth/2,-planetHeight/2, planetLenght/2,-planetWidth/2,planetHeight/2,//left
                planetLenght/2,planetWidth/2,-planetHeight/2, -planetLenght/2,planetWidth/2,-planetHeight/2, planetLenght/2,planetWidth/2,planetHeight/2, -planetLenght/2,planetWidth/2,planetHeight/2, -planetLenght/2,planetWidth/2,-planetHeight/2, planetLenght/2,planetWidth/2,planetHeight/2//right
            };
            //_vertexBufferObject = GL.GenBuffer();
            planetShader = new Shader("./Shaders/shader.vert", "./Shaders/shader.frag");
        }

        public void load()
        {
            //GL.Enable(EnableCap.DepthTest);
            //GL.DepthFunc(DepthFunction.Less);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            planetShader.Use();
            GL.GenVertexArrays(1, out vaoPlanet);
            GL.BindVertexArray(vaoPlanet);
            GL.GenBuffers(1, out planetBufferObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, planetBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, planet.Length * sizeof(float), planet, BufferUsageHint.StaticDraw);

            //var vertexLocation = _frameshader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        }

        public void render(FrameEventArgs e, Matrix4 model)
        {
            //_frameshader.SetMatrix4("model", model);

            planetShader.Use();
            planetShader.SetMatrix4("view", Program.camera.GetViewMatrix());
            planetShader.SetMatrix4("projection", Program.camera.GetProjectionMatrix());
            planetShader.SetVector3("objectColor", new Vector3(0.160f, 1f, 0.572f));
            planetShader.SetMatrix4("model", model);
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);//закрашивать или оставить каркас
            GL.BindVertexArray(vaoPlanet);
            GL.DrawArrays(PrimitiveType.Triangles, 0, planet.Length);
        }

        public void destroy(EventArgs e)
        {
            GL.DeleteProgram(planetShader.Handle);
        }

    }
}
