using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

namespace Tycoon
{
    public class KitchenPlateController : MonoBehaviour, IDropHandler
    {
        public GameObject resultFoodImage;
        public GameObject parrotHand;
        public GameObject parrotHandAngry;

        List<IngredientSequence> ingredients = new List<IngredientSequence>();
        bool canDrop = true;
        bool hasFood = false;
        string recipeName = "";

        private List<GameObject> tempIngredients = new List<GameObject>();

        public void OnDrop(PointerEventData eventData)
        {
            try
            {
                if (transform.childCount <= 4) canDrop = true;

                if (canDrop != true) return;

                if (eventData.pointerDrag.GetComponent<IngredientController>().isStarted == false)
                {
                    GameManager.instance.inventory[eventData.pointerDrag.name]--;
                    eventData.pointerDrag.GetComponent<IngredientController>().beforeBowl
                        .GetComponent<BeforeBowlController>().UpdateBowl();
                }

                if (transform.childCount > 4) canDrop = false;

                /*ingredients.Add(new IngredientSequence(
                        eventData.pointerDrag.name,
                        eventData.pointerDrag.GetComponent<IngredientController>().cookingSequences));

                    if (ingredients.Count > 5) canDrop = false;

                    tempIngredients.Add(eventData.pointerDrag);*/
                eventData.pointerDrag.GetComponent<IngredientController>().ingredientState =
                    IngredientController.IngredientState.Plated;
                eventData.pointerDrag.transform.SetParent(transform);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }


        AudioSource audioSource;
        public AudioClip ringingSound;

        public void OnServiceBellClicked()
        {
            audioSource.clip = ringingSound;
            audioSource.Play();

            if (hasFood)
            {
                StartCoroutine(ServeToBar(recipeName));

                return;
            }

            //if (ingredients.Count > 0)
            if (transform.childCount > 1)
            {
                canDrop = false;
                recipeName = "";

                /*foreach (var o in tempIngredients)
                {
                    Destroy(o);
                }*/
                for (int i = 1; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    ingredients.Add(new IngredientSequence(
                        child.name,
                        child.GetComponent<IngredientController>().cookingSequences));
                }

                for (int i = 1; i < transform.childCount; i++)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }

                foreach (KeyValuePair<string, Menu> recipe in GameManager.instance.menus)
                {
                    recipeName = recipe.Key;

                    if (recipe.Value.ingredientSequences.Count == ingredients.Count)
                    {
                        for (int i = 0; i < recipe.Value.ingredientSequences.Count; i++)
                        {
                            if (!recipe.Value.ingredientSequences[i].ingredientName
                                .Equals(ingredients[i].ingredientName))
                            {
                                recipeName = "";
                                break;
                            }

                            if (!recipe.Value.ingredientSequences[i].cookingSequences
                                .SequenceEqual(ingredients[i].cookingSequences))
                            {
                                recipeName = "";
                                break;
                            }
                        }
                    }
                    else
                    {
                        recipeName = "";
                    }


                    if (!recipeName.Equals(""))
                    {
                        break;
                    }
                }

                if (recipeName == "")
                {
                    // 쓰레기 완성
                    resultFoodImage.GetComponent<Image>().sprite =
                        Resources.Load<Sprite>("Dots/Tycoon/Foods/failedFood");
                    resultFoodImage.SetActive(true);

                    StartCoroutine(ThrowAway());
                }
                else if (!(GameManager.instance.menuOfTheMonth["1"].Contains(recipeName)
                           || GameManager.instance.menuOfTheMonth["2"].Contains(recipeName)
                           ||(GameManager.instance.menuOfTheMonth.TryGetValue("3", out var menuOfTheMonth3) && menuOfTheMonth3.Contains(recipeName))))
                {
                    resultFoodImage.GetComponent<Image>().sprite =
                        Resources.Load<Sprite>("Dots/Tycoon/Foods/" + recipeName);
                    resultFoodImage.SetActive(true);

                    StartCoroutine(ThrowAway());
                }
                else
                {
                    resultFoodImage.GetComponent<Image>().sprite =
                        Resources.Load<Sprite>("Dots/Tycoon/Foods/" + recipeName);
                    resultFoodImage.SetActive(true);
                    StartCoroutine(ServeToBar(recipeName));

                }

                ingredients.Clear();

            }
        }

        public float parrotHandSpeed;

        IEnumerator ServeToBar(string recipeName)
        {
            bool servingResult = BarManager.instance.GetServed(recipeName);

            if (servingResult == false)
            {
                hasFood = true;

                parrotHand.SetActive(true);
                parrotHandAngry.SetActive(true);
                Vector3 tempV = parrotHand.GetComponent<RectTransform>().anchoredPosition;
                parrotHand.GetComponent<RectTransform>().anchoredPosition = new Vector3(1038, tempV.y, tempV.z);

                yield return new WaitForSeconds(1f);
                parrotHand.SetActive(false);
                parrotHandAngry.SetActive(false);

            }
            else
            {
                parrotHand.SetActive(true);

                Vector3 tempV = parrotHand.GetComponent<RectTransform>().anchoredPosition;

                for (float x = 1586; x >= 720; x -= Time.deltaTime * parrotHandSpeed)
                {
                    parrotHand.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, tempV.y, tempV.z);
                    yield return null;
                }

                resultFoodImage.SetActive(false);

                for (float x = 720; x <= 1600; x += Time.deltaTime * parrotHandSpeed)
                {
                    parrotHand.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, tempV.y, tempV.z);
                    yield return null;
                }


                canDrop = true;
                hasFood = false;
            }

            yield return null;
        }

        IEnumerator ThrowAway()
        {
            parrotHand.SetActive(true);
            parrotHandAngry.SetActive(true);
            Vector3 tempV = parrotHand.GetComponent<RectTransform>().anchoredPosition;
            parrotHand.GetComponent<RectTransform>().anchoredPosition = new Vector3(1038, tempV.y, tempV.z);

            yield return new WaitForSeconds(2f);
            parrotHand.SetActive(false);
            parrotHandAngry.SetActive(false);

            resultFoodImage.SetActive(false);
            canDrop = true;

            yield return null;
        }

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.volume = GameManager.instance.settings.soundEffectsVolume;
        }
    }
}
