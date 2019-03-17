using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject block;
    public int[] cubeInfo = new int[3];
    public GameObject[,,] blockArray;
    public int[] selectedIndex;
    // 0: basic, 1: transparent, 2: suspected, 3:mine, 4: selected
    public Material[] mats = new Material[6];
    public int numMines;

    // Create blocks by given information
    void CreateBlocks(int a, int b, int c)
    {
        string debugLog = string.Format("CreateBlocks: {0}, {1}, {2}", a, b, c);
        Debug.Log(debugLog);

        GameObject cam;
        blockArray = new GameObject[a, b, c];
        bool[,,] mines = new bool[a, b, c];

        // Set all mine location
        System.Random r = new System.Random();
        /*
        while (true)
        {
            int cnt = 0;

            if (cnt == numMines)
            {
                break;
            }
            if(!mines[r.Next(a), r.Next(b), r.Next(c)])
            {
                mines[r.Next(a), r.Next(b), r.Next(c)] = true;
                cnt++;
            }
        }
        */

        // Create blocks
        for (int i = 0; i < a; i++)
        {
            for(int j = 0; j < b; j++)
            {
                for(int k = 0; k < c; k++)
                {
                    GameObject new_block = Instantiate(block, new Vector3(i, j, k), Quaternion.identity);
                    Block blk = new_block.GetComponent<Block>();
                    blk.flag = false;
                    blk.selected = false;
                    //blk.isMine = mines[i, j, k];
                    blk.isMine = false;
                    blk.discovered = false;
                    blk.number = 0;
                    if(0 < blk.number)
                        blk.numText.GetComponent<TextMesh>().text = Convert.ToString(blk.number);
                    blk.numText.SetActive(false);
                    SetBlockMaterial(new_block);
                    blockArray[i, j, k] = new_block;
                }
            }
        }

        // Locate camera position
        cam = GameObject.Find("Main Camera");
        cam.transform.position = new Vector3(2f * a, 2f * b , 2f * c);
        cam.transform.LookAt(new Vector3(a, b, c));

        SelectBlock(a-1, b-1, c-1);
    }

    // Makes block selected and change block material
    void SelectBlock(int x, int y, int z)
    {
        string debugLog = string.Format("SelectBlock");
        Debug.Log(debugLog);

        GameObject old_block = blockArray[selectedIndex[0], selectedIndex[1], selectedIndex[2]];
        GameObject new_block = blockArray[x, y, z];

        old_block.GetComponent<Block>().selected = false;
        SetBlockMaterial(old_block);

        new_block.GetComponent<Block>().selected = true;
        SetBlockMaterial(new_block);

        selectedIndex = new int[3] { x, y, z };
    }

    // Set block's material
    void SetBlockMaterial(GameObject block)
    {
        /*
        string debugLog = string.Format("SetBlockMaterial");
        Debug.Log(debugLog);
        */

        Block blk = block.GetComponent<Block>();

        if (blk.discovered && blk.isMine)
        {
            foreach (Transform child in block.transform)
            {
                if(child.name != "Number")
                {
                    child.gameObject.GetComponent<MeshRenderer>().material = mats[3];
                }
            }
        }
        else if (blk.selected)
        {
            
            if (blk.discovered)
            {
                foreach (Transform child in block.transform)
                {
                    if (child.name == "Number")
                    {
                        child.gameObject.SetActive(true);
                    }
                    else if (child.name == "Body")
                    {
                        child.gameObject.SetActive(true);
                        child.gameObject.GetComponent<MeshRenderer>().material = mats[4];
                    }
                    else
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }
            else if (blk.flag)
            {
                foreach (Transform child in block.transform)
                {
                    if (child.name != "Number")
                    {
                        child.gameObject.GetComponent<MeshRenderer>().material = mats[5];
                    }
                }
            }
            else
            {
                foreach (Transform child in block.transform)
                {
                    if (child.name != "Number")
                    {
                        child.gameObject.GetComponent<MeshRenderer>().material = mats[4];
                    }
                }
            }

        }
        else if (blk.discovered)
        {
            foreach (Transform child in block.transform)
            {
                if (child.name == "Number")
                {
                    child.gameObject.SetActive(true);
                }
                else if (child.name == "Body")
                {
                    child.gameObject.SetActive(true);
                    child.gameObject.GetComponent<MeshRenderer>().material = mats[1];
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
        else if (blk.flag)
        {
            foreach (Transform child in block.transform)
            {
                if (child.name != "Number")
                {
                    child.gameObject.GetComponent<MeshRenderer>().material = mats[2];
                }
            }
        }
        else // No select, No discovered, No flag => basic
        {
            foreach (Transform child in block.transform)
            {
                if (child.name != "Number")
                {
                    if (child.name == "Body")
                    {
                        child.gameObject.GetComponent<MeshRenderer>().material = mats[1];
                    }
                    else
                    {
                        child.gameObject.GetComponent<MeshRenderer>().material = mats[0];
                    }
                }
            }
        }
    }

    // Keyboard
    void Keyboard()
    {
        // W : Forward, S : Backward, A : Leftward, D : Rightward, Q : Upward, E : Downward
        try
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                SelectBlock(selectedIndex[0] - 1, selectedIndex[1], selectedIndex[2]);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                SelectBlock(selectedIndex[0] + 1, selectedIndex[1], selectedIndex[2]);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                SelectBlock(selectedIndex[0], selectedIndex[1], selectedIndex[2] - 1);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                SelectBlock(selectedIndex[0], selectedIndex[1], selectedIndex[2] + 1);
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                SelectBlock(selectedIndex[0], selectedIndex[1] + 1, selectedIndex[2]);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                SelectBlock(selectedIndex[0], selectedIndex[1] - 1, selectedIndex[2]);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                GameObject flagBlock = blockArray[selectedIndex[0], selectedIndex[1], selectedIndex[2]];
                flagBlock.GetComponent<Block>().flag = !flagBlock.GetComponent<Block>().flag;
                SetBlockMaterial(flagBlock);
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                GameObject discoverBlock = blockArray[selectedIndex[0], selectedIndex[1], selectedIndex[2]];
                discoverBlock.GetComponent<Block>().discovered = true;
                SetBlockMaterial(discoverBlock);
            }
        }
        catch(IndexOutOfRangeException e)
        {
            Debug.Log(e);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("GameManager start");
        // Basic setting
        Screen.SetResolution(640, 960, false);

        cubeInfo = new int[3] { 4, 4, 4 };
        selectedIndex = new int[3] { 0, 0, 0 };
        numMines = 3;

        for (int i = 0; i < 3; i++)
        {
            Debug.Assert(0 < cubeInfo[i] && cubeInfo[i] <= 10);
        }
        CreateBlocks(cubeInfo[0], cubeInfo[1], cubeInfo[2]);
    }

    // Update is called once per frame
    void Update()
    {
        Keyboard();
    }
}
