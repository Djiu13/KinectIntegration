using WindowsInput;

namespace RemotingInterfaces
{
    /// <summary>
    ///  Detaille les opérations possibles entre le client et le service kinect
    /// </summary>
    public interface IRemoteOperation
    {
        /// <summary>
        ///     Ecrit un message dans la console
        /// </summary>
        /// <param name="mess"> Le message</param>
        void message(string mess);
        /// <summary>
        ///  Synthethise un message en voix
        /// </summary>
        /// <param name="mess">Le message</param>
        void play(string mess);

        /// <summary>
        ///   Appuie sur une touche
        /// </summary>
        /// <param name="key">La touche</param>
        void press_key_down(VirtualKeyCode key);
        /// <summary>
        ///   Relache sur une touche
        /// </summary>
        /// <param name="key">La touche</param>
        void press_key_up(VirtualKeyCode key);

        /// <summary>
        ///     Fait bouger la souris à une positione donnée
        /// </summary>
        /// <param name="p"> souris.x</param>
        /// <param name="p_2">souris.y</param>
        void moveCursorToPosition(float p, float p_2);
        /// <summary>
        ///  Fait bouger la souris à gauche, droite ou centre de l'encran
        /// </summary>
        void moveCursorToLeft();
        void moveCursorToRight();
        void moveCursorToCenter();

        /// <summary>
        /// Appuie sur le bouton gauche de la souris
        /// </summary>
        void mouseLeftDown();
        /// <summary>
        /// Relache le bouton gauche de la souris
        /// </summary>
        void mouseLeftUp();
        /// <summary>
        ///  Realise un double click
        /// </summary>
        void doubleClick();
        /// <summary>
        /// Appuie sur le bouton droit de la souris
        /// </summary>
        void mouseRightDown();
        /// <summary>
        /// Relache le bouton droit de la souris
        /// </summary>
        void mouseRightUp();

        /// <summary>
        /// Execute un programme
        /// </summary>
        /// <param name="p"> Le programme</param>
        void execProgramm(string p);

    }
}
