
The config files must be copied to the binary folders. VS does that for you (see the build events).
If you are not using Visual Studio, remember to do it.

This sample
-----------

The ServerApp uses the Remoting facility to expose the Kernel remote component registry.

The ClientApp uses the Remoting facility to access the remote component registry. 


Running the sample
------------------

Start the ServerApp

Execute the ClientApp one or more times.

The output should be (on the ServerApp console):

--------------------------------------------
Server started, press enter to close.
Some message from the client
Some message from the client
--------------------------------------------
