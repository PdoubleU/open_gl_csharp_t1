using OpenTK;
using OpenTK.Graphics.OpenGL4;
using GLFW;
using GlmSharp;

using Shaders;
using Models;

namespace PMLabs
{
    //Implementacja interfejsu dostosowującego metodę biblioteki Glfw służącą do pozyskiwania adresów funkcji i procedur OpenGL do współpracy z OpenTK.
    public class BC : IBindingsContext
    {
        public IntPtr GetProcAddress(string procName)
        {
            return Glfw.GetProcAddress(procName);
        }
    }

    class Program
    {
        static Sphere sun = new Sphere(0.5f, 10.0f, 10.0f);
        static Sphere earth = new Sphere(0.2f, 10.0f, 10.0f);
        static Sphere moon = new Sphere(0.1f, 10.0f, 10.0f);
        public static void InitOpenGLProgram(Window window)
        {
            // Czyszczenie okna na kolor czarny
            GL.ClearColor(0, 0, 0, 1);

            // Ładowanie programów cieniujących
            DemoShaders.InitShaders("Shaders\\");
        }

        public static void DrawSceneSun(Window window, float time)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            mat4 V = mat4.LookAt(
                new vec3(0.0f, 0.0f, -3.0f),
                new vec3(0.0f, 0.0f, 0.0f),
                new vec3(0.0f, 1.0f, 0.0f));
            mat4 P = mat4.Perspective(glm.Radians(60.0f), 1.0f, 1.0f, 0.0f);

            DemoShaders.spConstant.Use();
            GL.UniformMatrix4(DemoShaders.spConstant.U("P"), 1, false, P.Values1D);
            GL.UniformMatrix4(DemoShaders.spConstant.U("V"), 1, false, V.Values1D);

            vec3 sunCenter = new vec3(0.0f, 0.0f, 0.0f);

            float earthOrbitRadius = 1.5f;
            float moonOrbitRadius = 0.5f;

            // Oblicz wspolrzedne polozenia ziemi na orbicie slonca
            float eAngle = glm.Radians(30.0f * time); // kąt obrotu planety
            float ePosZ = sunCenter.z + (float)Math.Cos(eAngle) * earthOrbitRadius;
            float ePosX = sunCenter.x + (float)Math.Sin(eAngle) * earthOrbitRadius;
            vec3 earthPosition = new vec3(ePosX, 0.0f, ePosZ);

            vec3 earthCenter = earthPosition;

            // Oblicz wspolrzedne polozenia księzyca na orbicie ziemi
            float mAngle = glm.Radians(120.0f * time); // kąt obrotu księzyca
            float mPosZ = earthCenter.z + (float)Math.Cos(mAngle) * moonOrbitRadius;
            float mPosX = earthCenter.x + (float)Math.Sin(mAngle) * moonOrbitRadius;
            vec3 moonPosition = new vec3(mPosX, 0.0f, mPosZ);

            mat4 M1 = mat4.Identity;
            M1 *= mat4.Translate(sunCenter); // ustawienie słonca na srodku
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, M1.Values1D);
            sun.drawWire(); // rysowanie slonca

            mat4 M2 = mat4.Identity;
            M2 *= mat4.Translate(earthPosition); // przeniesienie reprezentacji ziemi w wyliczone miejsce w ukladzie
            M2 *= mat4.Rotate(glm.Radians(180.0f * time), new vec3(0.0f, 1.0f, 0.0f)); // obrot wokol wlasnej osi
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, M2.Values1D);
            earth.drawWire(); // rysowanie ziemi

            mat4 M3 = mat4.Identity;
            M3 *= mat4.Translate(moonPosition);  // przeniesienie reprezentacji księzyca w wyliczone miejsce w ukladzie
            M3 *= mat4.Rotate(glm.Radians(-90.0f * time), new vec3(0.0f, 1.0f, 0.0f)); // obrot wokol wlasnej osi
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, M3.Values1D);
            moon.drawWire(); // rysowanie ksiezyca

            Glfw.SwapBuffers(window);
        }


        public static void FreeOpenGLProgram(Window window)
        {
            // Możesz dodać odpowiednie czyszczenie zasobów tutaj, jeśli jest to konieczne
        }

        static void Main(string[] args)
        {
            Glfw.Init();

            Window window = Glfw.CreateWindow(500, 500, "Programowanie multimedialne", GLFW.Monitor.None, Window.None);

            Glfw.MakeContextCurrent(window);
            Glfw.SwapInterval(1);

            GL.LoadBindings(new BC()); //Pozyskaj adresy implementacji poszczególnych procedur OpenGL

            InitOpenGLProgram(window);

            Glfw.Time = 0;

            while (!Glfw.WindowShouldClose(window))
            {
                DrawSceneSun(window, (float)Glfw.Time);
                Glfw.PollEvents();
            }

            FreeOpenGLProgram(window);

            Glfw.Terminate();
        }


    }
}