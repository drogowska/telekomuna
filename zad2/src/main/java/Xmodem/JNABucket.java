package Xmodem;

public class JNABucket {

        public static void main(String args[]) {
            JNAApiInterface jnaLib = JNAApiInterface.INSTANCE;
            jnaLib.printf("Hello World");
            String testName = null;

            for (int i = 0; i < args.length; i++) {
                jnaLib.printf("\nArgument %d : %s", i, args[i]);
            }

            jnaLib.printf("Please Enter Your Name:\n");
            jnaLib.scanf("%s", testName);
            jnaLib.printf("\nYour name is %s", testName);
        }

}
