using System;
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

namespace Snake
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int WIDTHWINDOW = 100;
        const int HEIGHTWINDOW = 100;
        const int PIXEL = 10;



        List<(int, int)> snake = new List<(int, int)>() 
        { 
            (1, 2), 
            (2, 2),
            (3, 2),
            (4, 2)
        };

        Dictionary<char, (int, int)> action = new Dictionary<char, (int, int)>()
        {
            ['L'] = (-1,0),
            ['U'] = (0,-1),
            ['R'] = (1,0),
            ['D'] = (0,1),
        };

        (int, int) apple;
        public static Random rand = new Random();
        TimeSpan timestart = DateTime.Now.TimeOfDay;
        Key last_key_down = Key.D;
        bool apple_eaten = false;


        public MainWindow()
        {
            InitializeComponent();

            apple = place_apple();
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        (int, int) place_apple()
        {
            apple = (rand.Next(0, HEIGHTWINDOW / PIXEL - 1), rand.Next(0, HEIGHTWINDOW / PIXEL - 1));

            foreach (var item in snake)
            {
                if (item == apple)
                {
                    return place_apple();
                }
            }
            return apple;
        }


        void rectangle_set_x_y((int, int) xy, bool isapple = false, bool ishead = false)
        {
            
            var brushes = isapple ? Brushes.Red : Brushes.White ;
            if (ishead)
                brushes = Brushes.Yellow;

            Rectangle rectangle = new Rectangle()
            {
                Width = PIXEL,
                Height = PIXEL,
                Fill = brushes,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            MainCanvas.Children.Add(rectangle);
            Canvas.SetLeft(rectangle, xy.Item1 * PIXEL);
            Canvas.SetTop(rectangle, xy.Item2 * PIXEL);
        }

        void CompositionTarget_Rendering(object sender, System.EventArgs e)
        {
            if (DateTime.Now.TimeOfDay - timestart >= TimeSpan.FromSeconds(0.5))
            {
                timestart = DateTime.Now.TimeOfDay;
                MainCanvas.Children.Clear();

                // apple collision snake
                if (snake[snake.Count-1] == apple)
                {
                    apple = place_apple();
                    apple_eaten = true; 
                }

                // place apple
                rectangle_set_x_y(apple, true);

                // move snake
                char act;
                if (last_key_down == Key.W)
                    act = 'U';
                else if (last_key_down == Key.A)
                    act = 'L';
                else if (last_key_down == Key.S)
                    act = 'D';
                else 
                    act = 'R';
                var head_snake = snake[snake.Count - 1];
                head_snake.Item1 += action[act].Item1;
                head_snake.Item2 += action[act].Item2;

                // collision with a snake before death
                bool collision_snake = snake.Find(x => x == head_snake) != (0, 0);
                bool check_x_and_snake = head_snake.Item1 <= WIDTHWINDOW / PIXEL - 1 & head_snake.Item1 >= 0;
                bool check_y_and_snake = head_snake.Item2 <= HEIGHTWINDOW / PIXEL - 1 & head_snake.Item2 >= 0;
                if (collision_snake || !check_x_and_snake || !check_y_and_snake)
                {
                    MessageBox.Show("Game Over");
                    CompositionTarget.Rendering -= CompositionTarget_Rendering;
                    return;
                }
                snake.Add(head_snake);
                if (!apple_eaten)
                    snake.Remove(snake[0]);
                apple_eaten = false;

                // draw snake
                foreach (var item in snake)
                {
                    if (item == snake[snake.Count - 1])
                    {
                        rectangle_set_x_y(item, false, true);
                        continue;
                    }
                    rectangle_set_x_y(item);
                }

                
            }
        }

        private void MainCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            var temp = new List<Key>() { Key.W, Key.A, Key.S, Key.D};
            if (temp.Contains(e.Key))
            {
                last_key_down = e.Key;
            }
        }
    }
}
