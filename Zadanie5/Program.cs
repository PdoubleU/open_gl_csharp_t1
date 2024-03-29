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
        static Sphere sun = new Sphere(0.5f, 20.0f, 20.0f);
        static Sphere earth = new Sphere(0.2f, 20.0f, 20.0f);
        static Sphere moon = new Sphere(0.1f, 20.0f, 20.0f);
        static Sphere earth2 = new Sphere(0.25f, 20.0f, 20.0f);
        static Sphere moon2 = new Sphere(0.07f, 20.0f, 20.0f);
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
            mat4 P = mat4.Perspective(glm.Radians(120.0f), 1.0f, 1.0f, 0.0f);

            DemoShaders.spConstant.Use();
            GL.UniformMatrix4(DemoShaders.spConstant.U("P"), 1, false, P.Values1D);
            GL.UniformMatrix4(DemoShaders.spConstant.U("V"), 1, false, V.Values1D);

            vec3 sunCenter = new vec3(0.0f, 0.0f, 0.0f);

            float earthOrbitRadius = 1.5f;
            float moonOrbitRadius = 0.5f;
            float earth2OrbitRadius = 2.0f;
            float moon2OrbitRadius = 0.3f;

            // Oblicz wspolrzedne polozenia ziemi na orbicie slonca
            float eAngle = glm.Radians(30.0f * time); // kąt obrotu planety
            float ePosZ = sunCenter.z + (float)Math.Cos(eAngle) * earthOrbitRadius;
            float ePosX = sunCenter.x + (float)Math.Sin(eAngle) * earthOrbitRadius;
            vec3 earthPosition = new vec3(ePosX, 0.0f, ePosZ);

            vec3 earthCenter = earthPosition;

            // Oblicz wspolrzedne polozenia ziemi 2 na orbicie slonca
            float e2Angle = glm.Radians(-30.0f * time); // kąt obrotu planety
            float e2PosY = sunCenter.y + (float)Math.Cos(e2Angle) * earth2OrbitRadius;
            float e2PosX = sunCenter.x + (float)Math.Sin(e2Angle) * earth2OrbitRadius;
            vec3 earth2Position = new vec3(e2PosX, e2PosY, 0.0f);

            vec3 earth2Center = earth2Position;

            // Oblicz wspolrzedne polozenia księzyca na orbicie ziemi
            float mAngle = glm.Radians(120.0f * time); // kąt obrotu księzyca
            float mPosZ = earthCenter.z + (float)Math.Cos(mAngle) * moonOrbitRadius;
            float mPosX = earthCenter.x + (float)Math.Sin(mAngle) * moonOrbitRadius;
            vec3 moonPosition = new vec3(mPosX, 0.0f, mPosZ);

            // Oblicz wspolrzedne polozenia księzyca 2 na orbicie ziemi 2
            float m2Angle = glm.Radians(60.0f * time); // kąt obrotu księzyca
            float m2PosY = earth2Center.y + (float)Math.Cos(m2Angle) * moon2OrbitRadius;
            float m2PosX = earth2Center.x + (float)Math.Sin(m2Angle) * moon2OrbitRadius;
            vec3 moon2Position = new vec3(m2PosX, m2PosY, 0.0f);

            mat4 SUN = mat4.Identity;
            SUN *= mat4.Translate(sunCenter); // ustawienie słonca na srodku
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, SUN.Values1D);
            sun.drawWire(); // rysowanie slonca

            mat4 EARTH1 = mat4.Identity;
            EARTH1 *= mat4.Translate(earthPosition); // przeniesienie reprezentacji ziemi w wyliczone miejsce w ukladzie
            EARTH1 *= mat4.Rotate(glm.Radians(180.0f * time), new vec3(0.0f, 1.0f, 0.0f)); // obrot wokol wlasnej osi
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, EARTH1.Values1D);
            earth.drawWire(); // rysowanie ziemi

            mat4 MOON1 = mat4.Identity;
            MOON1 *= mat4.Translate(moonPosition);  // przeniesienie reprezentacji księzyca w wyliczone miejsce w ukladzie
            MOON1 *= mat4.Rotate(glm.Radians(-90.0f * time), new vec3(0.0f, 1.0f, 0.0f)); // obrot wokol wlasnej osi
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, MOON1.Values1D);
            moon.drawWire(); // rysowanie ksiezyca

            mat4 EARTH2 = mat4.Identity;
            EARTH2 *= mat4.Translate(earth2Position); // przeniesienie reprezentacji ziemi 2 w wyliczone miejsce w ukladzie
            EARTH2 *= mat4.Rotate(glm.Radians(180.0f * time), new vec3(0.0f, 0.0f, 1.0f)); // obrot wokol wlasnej osi
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, EARTH2.Values1D);
            earth2.drawWire(); // rysowanie ziemi 2

            mat4 MOON2 = mat4.Identity;
            MOON2 *= mat4.Translate(moon2Position); // przeniesienie reprezentacji ksiezyca 2 w wyliczone miejsce w ukladzie
            MOON2 *= mat4.Rotate(glm.Radians(-30.0f * time), new vec3(1.0f, 0.0f, 0.0f)); // obrot wokol wlasnej osi
            GL.UniformMatrix4(DemoShaders.spConstant.U("M"), 1, false, MOON2.Values1D);
            moon2.drawWire(); // rysowanie księzyca 2

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