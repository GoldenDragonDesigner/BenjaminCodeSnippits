# BenjaminCodeSnippits
This Repository is for those that would like to view the code that I have worked on over the years

In this Repository it is setup in a way to show the progression of how I built my AI system for my capstone project and also in Brushlings Pale Moon.

Please do not edit or make use of these scripts without asking for permission as I have spent a lot of time on them and would like to preserve what I have done.

BB_GuardStateMachine.cs is the first introduction for how an AI system can be setup in Unity3D using the Nav Mesh.  I used BB_GuardStateMachine as a referenece to create DirtySlug.cs.  I made the AI move around the screen with Nav Points.

BaseEnemy_1.cs was the first AI script that I built from the ground up.  I used it in my Capstone project in Phoenix Rising.  Boss.cs inherites from BaseEnemy.cs and makes use of alot of functionality and I also added functionality that was just for the Boss.

I then took the BaseEnemy_1.cs script and imported it to Brushlings Pale Moon with a key difference I did not have to make the basic enemies utilize nav Points they were free to roam and BaseEnemyScript.cs was born.  FireBoss.cs is an example of using inheritance for the method functionalities that I needed in order to make the FireBoss.cs work.

I also used a type of singleton pattern called GlobalVariables.cs and it made use of the static proterty so.  I set up the FireBoss.cs and FireBossManager.cs using the GlobalVariables.cs singleton pattern by going into the FireBoss.cs, for example, and in the awake function I put GlobalVariables.FireBoss = this; so that I could use the script without having to use the get component function or find object of type function which i found can be pretty performance heavy. 

Last but not least I decided to return to the DirtySlug.cs script and see if I could incorporate my BaseEnemyScript.cs script to it and after a few tweaks I managed to be able to do so, with some slight modifications I wanted to be able to see if I could make everything in each of the scripts that inheritated from BaseEnemy.cs, IE AnotherChildOfBaseEnemy.cs and ChildOfBaseEnemy.cs, and if you take a look I used custom written getters and setters and hard coded each of the child classes with specific specs for the variables.  I only did that so that I could actually see the use of Abstraction, Inheritance, Encapulation and Polymorphism.  The way that I was able to determine that I did use the 4 pillars was like this.  I Encapulated all the necessary information into BaseEnemy.cs and then made most of the variables protected, knowing that they would not be seen in the inspector of Unity and thus used Abstraction by using public getters and setters to be able to manipulate the data.  Inheritance was used because it helped me to not repeat code I just had to set the variables.  Polymorphism was used because I was overriding the parent class.


