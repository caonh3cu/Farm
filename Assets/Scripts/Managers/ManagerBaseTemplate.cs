
namespace CAT
{
    public class ManagerBaseTemplate<T> : ManagerBase where T:ManagerBaseTemplate<T>
    {
        private static T _instance;

        public static bool initialized = _instance != null;
        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    if (game_manager_object)
                    {
                        _instance = game_manager_object.GetComponent<T>();
                        if (!_instance)
                        {
                            _instance = game_manager_object.AddComponent<T>();
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                return _instance;
            }
        }
    }
}