using VirtualPadUtils.CommUtils;

public class InputState {

        public bool APressed {get; private set;} = false;
        public bool BPressed {get; private set;} = false;
        public bool StickPressed {get; private set;} = false;
        public int StickDir {get; private set;} = 0;
        public int StickMagnitude {get; private set;} = 0;

        private bool aReleased = false;
        private bool bReleased = false;
        private bool stickReleased = false;

        public InputState(){
        }

        public InputState ProcessNewData(InputData inputData){
            APressed = inputData.AButtonDown;
            BPressed = inputData.BButtonDown;
            APressed = APressed ? true : inputData.AButtonDown;
            aReleased = !inputData.AButtonDown;
            BPressed = BPressed ? true : inputData.BButtonDown;
            bReleased = !inputData.BButtonDown;
            StickPressed = StickPressed ? true : inputData.StickActive;
            stickReleased = !inputData.StickActive;
            StickDir = inputData.StickDir;
            return this;
        }

        public InputState NewFrame(){
            APressed = aReleased ? false : APressed;
            BPressed = bReleased ? false : BPressed;
            StickPressed = stickReleased ? false : StickPressed;
            aReleased = false;
            bReleased = false;
            stickReleased = false;
            return this;
        }

        public InputState Clone(){
            return (InputState) this.MemberwiseClone();
        }
    }