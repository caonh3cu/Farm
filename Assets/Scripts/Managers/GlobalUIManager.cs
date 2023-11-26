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
        
        public static LoadingUI loading;
        public GameObject loading_prefab;

        public static MainScreen main_screen;
        public GameObject main_screen_prefab;

        public static EscPopupUI esc;
        public GameObject esc_prefab;

        public static SwitchMagicUI switch_magic;
        public GameObject switch_magic_prefab;

        public static MapUI map;
        public GameObject map_prefab;

        public static BagUI bag;
        public GameObject bag_prefab;

        public static BlockInfoUI block_info;
        public GameObject block_info_prefab;
        
        private void Start()
        {
            esc = Instantiate(esc_prefab, canvas).GetComponent<EscPopupUI>();
            esc.Hide();

            loading = Instantiate(loading_prefab, canvas).GetComponent<LoadingUI>();
            loading.Hide();
            
            main_screen = Instantiate(main_screen_prefab, canvas).GetComponent<MainScreen>();
            main_screen.Hide();
            
            switch_magic = Instantiate(switch_magic_prefab, canvas).GetComponent<SwitchMagicUI>();
            switch_magic.Hide();
            
            map = Instantiate(map_prefab, canvas).GetComponent<MapUI>();
            map.Hide();
            
            bag = Instantiate(bag_prefab, canvas).GetComponent<BagUI>();
            bag.Hide();
            
            block_info = Instantiate(block_info_prefab, canvas).GetComponent<BlockInfoUI>();
            block_info.Hide();
            
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