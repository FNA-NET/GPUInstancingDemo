using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GPUInstancing
{
    public class Game1 : Game
    {
        struct SpriteInfo
        {
            public float Width;
            public float Height;
            public float X;
            public float Y;
            public Vector4 Diffuse;
        }

        VertexBuffer _vertexBuffer;
        IndexBuffer _indexBuffer;
        VertexBuffer _instanceBuffer;
        VertexDeclaration _instanceVertexDeclaration;
        VertexBufferBinding[] _bindings;
        SpriteInfo[] _instances;

        int _instanceCount = 20000;

        DateTime _nextUpdateTime;

        Matrix _projection;

        Texture2D _texture1;
        Effect _instancingEffect;

        Random _rand = new Random();

        public Game1()
        {
            GraphicsDeviceManager gdm = new GraphicsDeviceManager(this);

            // Typically you would load a config here...
            gdm.PreferredBackBufferWidth = 1280;
            gdm.PreferredBackBufferHeight = 720;
            gdm.IsFullScreen = false;
            gdm.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = false;

            IsMouseVisible = true;

            // All content loaded will be in a "Content" folder
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            /* This is a nice place to start up the engine, after
            * loading configuration stuff in the constructor
            */
            base.Initialize();
        }

        private void UpdateInstances()
        {
            for (int i = 0; i < _instanceCount; ++i)
            {
                var scale = _rand.NextDouble();
                var width = (int)(_texture1.Width * scale);
                var height = (int)(_texture1.Height * scale);
                _instances[i].Width = width;
                _instances[i].Height = height;
                _instances[i].X = _rand.Next(0, GraphicsDevice.Viewport.Width);
                _instances[i].Y = _rand.Next(0, GraphicsDevice.Viewport.Height);
                _instances[i].Y = _rand.Next(0, GraphicsDevice.Viewport.Height);
                _instances[i].Diffuse =
                    new Vector4(
                        (float)_rand.NextDouble(),
                        (float)_rand.NextDouble(),
                        (float)_rand.NextDouble(),
                        (float)_rand.NextDouble());
            }

            _instanceBuffer.SetData(_instances);
        }

        protected override void LoadContent()
        {
            _texture1 = Content.Load<Texture2D>("Image1");
            _instancingEffect = Content.Load<Effect>("Effects/GPUInstancing");

            _projection = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, 0, 0F, 100F);

            {
                var vertices = new VertexPositionTexture[4];

                vertices[0] = new VertexPositionTexture(new Vector3(0, 1, 0), new Vector2(0, 1));
                vertices[1] = new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 1));
                vertices[2] = new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0, 0));
                vertices[3] = new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(1, 0));

                _vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionTexture), 4, BufferUsage.WriteOnly);
                _vertexBuffer.SetData<VertexPositionTexture>(vertices);
            }

            {
                _indexBuffer = new IndexBuffer(this.GraphicsDevice, typeof(ushort), 6, BufferUsage.WriteOnly);
                _indexBuffer.SetData(new ushort[] {
                    2, 1, 0,
                    3, 1, 2
                });
            }

            {
                VertexElement[] streamElements = new VertexElement[2];
                // float4
                streamElements[0] = new VertexElement(0, VertexElementFormat.Vector4,
                            VertexElementUsage.Position, 1);
                streamElements[1] = new VertexElement(sizeof(float) * 4, VertexElementFormat.Vector4,
                            VertexElementUsage.Color, 1);
                _instanceVertexDeclaration = new VertexDeclaration(streamElements);

                _instanceBuffer = new VertexBuffer(this.GraphicsDevice, _instanceVertexDeclaration,
                    _instanceCount, BufferUsage.WriteOnly);

                _instances = new SpriteInfo[_instanceCount];

                UpdateInstances();
            }

            _bindings = new VertexBufferBinding[2];
            _bindings[0] = new VertexBufferBinding(_vertexBuffer);
            _bindings[1] = new VertexBufferBinding(_instanceBuffer, 0, 1);

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            // Clean up after yourself!
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (_nextUpdateTime < DateTime.Now)
            {
                UpdateInstances();
                _nextUpdateTime = DateTime.Now.AddSeconds(1);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.Textures[0] = _texture1;
            GraphicsDevice.SetVertexBuffers(_bindings);
            GraphicsDevice.Indices = _indexBuffer;

            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            _instancingEffect.Parameters["Projection"].SetValue(_projection);
            _instancingEffect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 6, 0, 2, _instanceCount);

            base.Draw(gameTime);
        }
    }
}
