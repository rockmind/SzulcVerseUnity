using System.Collections;
using System.Collections.Generic;
    using UnityEngine;

    public class OrbitingScript : MonoBehaviour
    {
        public Transform centralStar;
        public Transform[] planets;
        public float rotationSpeed;
        
        void Start()
        {
            float[] scales = { 1.5F, 0.1925F, 0.4789F, .5F, 0.2635F, 0.8092F, 0.7094F, .6074F, .5827F };
            Vector3[] positions =
            {
                new Vector3(0,0,0),
                new Vector3(2.65312052F,0,2.34210825F),
                new Vector3(-0.861534119F,0,4.39841986F),
                new Vector3(5.25838089F,0,2.26891756F),
                new Vector3(0.170938164F,0,6.86587572F),
                new Vector3(-2.14073706F,0,6.68878365F),
                new Vector3(7.53251266F,0,2.53298545F),
                new Vector3(-4.83697987F,0,7.20735645F),
                new Vector3(4.64297962F,0,8.10192394F)
            };
            centralStar.transform.position.Set(positions[0].x, positions[0].y, positions[0].z);
            centralStar.localScale = new Vector3(scales[0], scales[0], scales[0]);
            var i = 1;
            foreach (var planet in planets)
            {
                planet.transform.position = positions[i];
                var scaleVector = new Vector3(scales[i], scales[i], scales[i]);
                planet.localScale = scaleVector;
                ++i;
            }
            
        }

        void Update()
        {
            centralStar.Rotate(0,rotationSpeed*Time.deltaTime,0);
            foreach (var planet in planets)
            {
                planet.Rotate(0,rotationSpeed*Time.deltaTime*Random.Range(0.5F, 40F),0);
                planet.RotateAround(centralStar.transform.position, new Vector3(0,1,0), rotationSpeed * Time.deltaTime * Random.Range(.2F, 10F));
            }
            
        }
    }   
