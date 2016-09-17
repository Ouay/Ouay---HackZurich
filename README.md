# Ouay at HackZurich

Our goal is to prevent accidents by doing our best in making the everyday lives of the older members of the family safer and easier. We learn their habits by monitoring their activity around the house as well as their exit and return times. If anything unusual happens we will be able to immediatly notify relatives or even call for help.

It's not just about learning their habits but also about preventing accidents by remiding them about their medication or particular weather.

### Hardware
The core of the Ouay is a RaspberryPi 3 with a couple sensors and devices hooked up to it:

- Motion sensor
- Push button
- Microphone
- Speaker
- Relay to control a light

### Implementation
Through the BlueMix services we set up API calls with Node-RED to query and populate cloudant databases storing data on entry times of the user in their home, results of the data analysis and notifications. The analysis was done using a Notebook and consists of a simple median and std calculation of entry time for each day of the week. If the user happens to come home earlier or later than expected (outside of the range established by the std) an alert is sent to all listening phones (relatives/friends). If a user announces that he/she is leaving for a certain amount of time, a timer tracks for how long they have been away and fires an alert in case they are not home on time. 

### More code
[Notebook](https://console.ng.bluemix.net/data/notebooks/fac9f6cf-45f3-4fa7-bc09-081ed7127c45/view?access_token=3c6b927fb8c763bca8edf94afde2cd5a2cf8194a709c6cbeb31fa10cf58e8809)
Node-RED nodes
![Node-RED](https://cloud.githubusercontent.com/assets/3426517/18611394/e00f942a-7d37-11e6-87db-ef3f7e934334.png)


