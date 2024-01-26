using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CAT
{
    public class GlobalUIManager : ManagerBaseTemplate<GlobalUIManager>
    {
        public Transform canvas;
        
        public static LoadingUI loading => instance._loading;
        private LoadingUI _loading;
        public GameObject loading_prefab;

        public static MainScreen main_screen => instance._main_screen;
        private MainScreen _main_screen;
        public GameObject main_screen_prefab;

        public static EscPopupUI esc => instance._esc;
        private EscPopupUI _esc;
        public GameObject esc_prefab;

        public static SwitchMagicUI switch_magic => instance._switch_magic;
        private SwitchMagicUI _switch_magic;
        public GameObject switch_magic_prefab;

        public static MapUI map => instance._map;
        private MapUI _map;
        public GameObject map_prefab;

        public static BagUI bag => instance._bag;
        private BagUI _bag;
        public GameObject bag_prefab;

        public static BlockInfoUI block_info => instance._block_info;
        private BlockInfoUI _block_info;
        public GameObject block_info_prefab;

        public static PickTips pick_tips => instance._pick_tips;
        private PickTips _pick_tips;
        public GameObject pick_tips_prefab;
        
        private void Start()
        { 
            _esc = Instantiate(esc_prefab, canvas).GetComponent<EscPopupUI>();
            _esc.Hide(); 

            _loading = Instantiate(loading_prefab, canvas).GetComponent<LoadingUI>();
            _loading.Hide();
            
            _main_screen = Instantiate(main_screen_prefab, canvas).GetComponent<MainScreen>();
            _main_screen.Hide();
            
            _switch_magic = Instantiate(switch_magic_prefab, canvas).GetComponent<SwitchMagicUI>();
            _switch_magic.Hide();
            
            _map = Instantiate(map_prefab, canvas).GetComponent<MapUI>();
            _map.Hide();
            
            _bag = Instantiate(bag_prefab, canvas).GetComponent<BagUI>();
            _bag.Hide();
            
            _block_info = Instantiate(block_info_prefab, canvas).GetComponent<BlockInfoUI>();
            _block_info.Hide();

            _pick_tips = Instantiate(pick_tips_prefab, canvas).GetComponent<PickTips>();
            _pick_tips.Hide();
        }
    
        private void Update()
        {

            if (G.control_right)
            {
                if (Input.GetButtonDown("OpenBag"))
                {
                    if (bag.is_show)
                    {
                        bag.Hide();
                        main_screen.Show();
                    }
                    else
                    {
                        main_screen.Hide();
                        esc.Hide();
                        switch_magic.Hide();
                        map.Hide();
                        block_info.Hide();
                        // bag.Hide();
                        
                        bag.Show();
                    }
                }
                if (Input.GetButtonDown("OpenMap"))
                {
                    if (map.is_show)
                    {
                        map.Hide();
                        main_screen.Show();
                    }
                    else
                    {
                        main_screen.Hide();
                        esc.Hide();
                        switch_magic.Hide();
                        // map.Hide();
                        block_info.Hide();
                        bag.Hide();
                        
                        map.Show();
                    }
                }
                if (Input.GetButtonDown("Cancel"))
                {
                    if (main_screen.is_show)
                    {
                        if (block_info.is_show || switch_magic.is_show)
                        {
                            block_info.Hide();
                            switch_magic.Hide();
                        }
                        else
                        {
                            main_screen.Hide();
                            esc.Show();
                        }
                    }
                    else if (esc.is_show)
                    {
                        esc.Hide();
                        main_screen.Show();
                    }
                    else
                    {
                        main_screen.Hide();
                        // esc.Hide();
                        switch_magic.Hide();
                        map.Hide();
                        block_info.Hide();
                        bag.Hide();
                        
                        esc.Show();
                    }
                }
                if (Input.GetButtonDown("SwitchMagic"))
                {
                    if (main_screen.is_show)
                    {
                        if(switch_magic.is_show)
                            switch_magic.Hide(); 
                        else
                            switch_magic.Show();
                    }
                }
                if (Input.GetButtonDown("ShowBlockInfo"))
                {
                    if (main_screen.is_show)
                    {
                        if(block_info.is_show)
                            block_info.Hide();
                        else
                            block_info.Show();
                    }
                }
                // if (Input.GetKeyDown("ShowItemInfo"))
                // {
                //
                // }   
            }
        }
    }

}