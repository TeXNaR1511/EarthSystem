using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

namespace EarthSystem
{
    class Program : GameWindow
    {
        
        public static Camera camera;

        private Stopwatch timer;

        private bool _firstMove = true;
        private Vector2 _lastPos;

        Planet Earth;
        Sputnik Moon;
        Sputnik SC1;
        Sputnik SC2;

        private Vector3 earthPosition = new Vector3(0f, 0f, 0f);
        private Vector3 moonPosition = new Vector3(0f, 10f, 0f);
        private Vector3 SC1Position = new Vector3(0f, 10f, 10f);
        private Vector3 SC2Position = new Vector3(0f, 20f, 10f);

        public Program()
            : base(800, 600, GraphicsMode.Default, "MoonSurface")
        {
            WindowState = WindowState.Maximized;//формат окна
        }

        static void Main(string[] args)
        {
            
            using (Program program = new Program())
            {
                program.Run();
            }
            //Console.WriteLine()
            //Console.WriteLine(MathHelper.RadiansToDegrees(Math.Acos(0.8)) + " " + MathHelper.RadiansToDegrees(Math.Acos(-0.5)));
        }

        

        protected override void OnLoad(EventArgs e)
        {
            timer = new Stopwatch();
            timer.Start();

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            //GL.Enable(EnableCap.DepthTest);
            //GL.DepthFunc(DepthFunction.Less);
            //_vertexBufferObject = GL.GenBuffer();
            //GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            //GL.ClearColor(0.101f, 0.98f, 1.0f, 1.0f);
            GL.ClearColor(0.254f, 0.290f, 0.298f, 1.0f);
            //GL.ClearColor(0f, 0f, 1f,1f);
            //GL.Enable(EnableCap.DepthTest);

            
            

            //terrain = new Terrain(new FileInfo("./Resources/moon_surface.png"));//сама картинка
            camera = new Camera(new Vector3(20, 20, 20), Width / (float)Height);//положение камеры начальное
            ////textFrame = new TextFrame();
            //car = new Car();
            Earth = new Planet();
            Moon = new Sputnik();
            SC1 = new Sputnik();
            SC2 = new Sputnik();

            Earth.load();
            Moon.load();
            SC1.load();
            SC2.load();
            
            CursorVisible = false;
            //
            //carPosition.X = terrain.returnInitialCoord().X;
            //carPosition.Z = terrain.returnInitialCoord().Y;

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            double timeValue = timer.Elapsed.TotalSeconds;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            //textFrame.load();
            //car.load();
            //terrain.load();

            /*короче вывод такой, вот если у тебя есть точка ты ты хочешь узнать координаты
            после преобразования тогда надо умножить эту точку на ту же матрицу*/
            /*нужно давать изначальную точку*/

            //типо вот так
            /*Console.WriteLine(moonPosition+Moon.returnCameraPos());
            Console.WriteLine(new Vector4(moonPosition+Moon.returnCameraPos(), 1) * Matrix4.CreateRotationX((float)timeValue / 2));*/

            //textFrame = new TextFrame();

            //var transform1 = Matrix4.Identity * Matrix4.CreateTranslation(earthPosition);
            //var transform2 = Matrix4.Identity * Matrix4.CreateRotationZ((float)timeValue) * Matrix4.CreateTranslation(moonPosition);
            //var transform1 = transform;
            //var transform2 = transform;
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            var earthTransform = Matrix4.Identity * Matrix4.CreateTranslation(earthPosition);
            var moonTransform = Matrix4.Identity * Matrix4.CreateRotationX((float)timeValue / 2) * Matrix4.CreateTranslation(moonPosition);
            var SC1Transform = Matrix4.Identity * Matrix4.CreateRotationZ((float)timeValue / 4) * Matrix4.CreateTranslation(SC1Position);
            var SC2Transform = Matrix4.Identity * Matrix4.CreateRotationX((float)timeValue / 3) * Matrix4.CreateTranslation(SC2Position);

            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            Earth.render(e, earthTransform);
            Moon.render(e, moonTransform);
            SC1.render(e, SC1Transform );
            SC2.render(e, SC2Transform);

            Vector3[] allSPos = new Vector3[] { (new Vector4(0, 0, 0, 1) * moonTransform).Xyz, (new Vector4(0, 0, 0, 1) * SC1Transform).Xyz, (new Vector4(0, 0, 0, 1) * SC2Transform).Xyz };
            //Console.WriteLine((new Vector4(moonPosition, 1) * moonTransform).Xyz + " " + (new Vector4(SC1Position, 1) * SC1Transform).Xyz + " " + (new Vector4(SC2Position, 1) * SC2Transform).Xyz + " " + (new Vector4(moonPosition + Moon.returnCameraPos(), 1) * moonTransform).Xyz);
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Number of visible sputniks from Moon is " + Moon.numberVisibleSputnik(allSPos, (new Vector4(0, 0, 0, 1) * moonTransform).Xyz, (new Vector4(Moon.returnCameraPos(), 1) * moonTransform).Xyz));
            Console.WriteLine("Number of visible sputniks from SC1 is " + Moon.numberVisibleSputnik(allSPos, (new Vector4(0, 0, 0, 1) * SC1Transform).Xyz, (new Vector4(SC1.returnCameraPos(), 1) * SC1Transform).Xyz));
            Console.WriteLine("Number of visible sputniks from SC2 is " + Moon.numberVisibleSputnik(allSPos, (new Vector4(0, 0, 0, 1) * SC2Transform).Xyz, (new Vector4(SC2.returnCameraPos(), 1) * SC2Transform).Xyz));
            //Console.Clear();
            //Vector4 exp = new Vector4(moonPosition, 1);

            //Console.WriteLine(moonPosition+Moon.returnCameraPos());
            //Console.WriteLine((new Vector4(Moon.returnCameraPos(), 1) * moonTransform).Xyz);

            //Console.WriteLine(moonPosition + Moon.returnCameraPos());
            //Console.WriteLine((Matrix4.Identity * Matrix4.CreateRotationY((float)timeValue / 2) * Matrix4.CreateTranslation(moonPosition+Moon.returnCameraPos())));
            //carPosition.Y = terrain.returnHeightOnTriangle(new Vector2(carPosition.X, carPosition.Z));
            //transform1 *= Matrix4.CreateTranslation(carPosition);
            //transform1 *= Matrix4.CreateRotationZ(camera.return_pitch());
            //textFramePosition = camera.returnFront();
            //transform2 *= Matrix4.CreateTranslation(textFramePosition);
            //transform2 *= Matrix4.CreateTranslation(camera.Position + camera.Front);
            //transform2 *= Matrix4.CreateRotationZ(camera.return_pitch());
            //transform2*= Matrix4.CreateRotationZ(camera.return_yaw());

            //if (isForwardX && terrain.isObstacleForwardX(carPosition)) Console.WriteLine("Впереди по X препятствие");
            //if (isForwardY && terrain.isObstacleForwardY(carPosition)) Console.WriteLine("Впереди по Y препятствие");
            //if (!isForwardX && terrain.isObstacleBackX(carPosition)) Console.WriteLine("Сзади по X препятствие");
            //if (!isForwardY && terrain.isObstacleBackY(carPosition)) Console.WriteLine("Сзади по Y препятствие");

            //if(textFramePaint) textFrame.render(e, transform2);
            //car.render(e, transform1);
            //terrain.render(e, transform);
            //Console.WriteLine(carPosition+"\x020"+camera.Position);
            //Console.WriteLine(textFramePosition + "\x020" + camera.Position);
            //Console.WriteLine(camera.Front + "\x020" + camera.returnFront());
            //transform *= Matrix4.CreateTranslation(new Vector3(camera.Position.X-10,camera.Position.Y,camera.Position.Z-10));//положение окошка перед камерой
            //if (framePaint) textFrame.render(e, transform);
            //textFrame.render(e, transform);

            SwapBuffers();

            base.OnRenderFrame(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            const float cameraSpeed = 10f;
            const float sensitivity = 0.2f;

            if (input.IsKeyDown(Key.W))
            {
                camera.Position += camera.Front * cameraSpeed * (float)e.Time; 
            }

            if (input.IsKeyDown(Key.S))
            {
                camera.Position -= camera.Front * cameraSpeed * (float)e.Time; 
            }
            if (input.IsKeyDown(Key.A))
            {
                camera.Position -= camera.Right * cameraSpeed * (float)e.Time; 
            }
            if (input.IsKeyDown(Key.D))
            {
                camera.Position += camera.Right * cameraSpeed * (float)e.Time; 
            }
            if (input.IsKeyDown(Key.Space))
            {
                camera.Position += camera.Up * cameraSpeed * (float)e.Time; 
            }
            if (input.IsKeyDown(Key.LShift))
            {
                camera.Position -= camera.Up * cameraSpeed * (float)e.Time; 
            }
            
            var mouse = Mouse.GetState();

            if (_firstMove) 
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                
                camera.Yaw += deltaX * sensitivity;
                camera.Pitch -= deltaY * sensitivity; 
            }

            base.OnUpdateFrame(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (Focused) 
            {
                Mouse.SetPosition(X + Width / 2f, Y + Height / 2f);
            }

            base.OnMouseMove(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            camera.AspectRatio = Width / (float)Height;
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
            Earth.destroy(e);
            Moon.destroy(e);
            SC1.destroy(e);
            SC2.destroy(e);
            //car.destroy(e);
            //GL.DeleteBuffer(_vertexBufferObject);
            base.OnUnload(e);
        }
    }
}


