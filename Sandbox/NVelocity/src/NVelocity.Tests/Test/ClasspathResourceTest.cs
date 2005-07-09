namespace org.apache.velocity.test
{
    /*
    * The Apache Software License, Version 1.1
    *
    * Copyright (c) 2001 The Apache Software Foundation.  All rights
    * reserved.
    *
    * Redistribution and use in source and binary forms, with or without
    * modification, are permitted provided that the following conditions
    * are met:
    *
    * 1. Redistributions of source code must retain the above copyright
    *    notice, this list of conditions and the following disclaimer.
    *
    * 2. Redistributions in binary form must reproduce the above copyright
    *    notice, this list of conditions and the following disclaimer in
    *    the documentation and/or other materials provided with the
    *    distribution.
    *
    * 3. The end-user documentation included with the redistribution, if
    *    any, must include the following acknowlegement:
    *       "This product includes software developed by the
    *        Apache Software Foundation (http://www.apache.org/)."
    *    Alternately, this acknowlegement may appear in the software itself,
    *    if and wherever such third-party acknowlegements normally appear.
    *
    * 4. The names "The Jakarta Project", "Velocity", and "Apache Software
    *    Foundation" must not be used to endorse or promote products derived
    *    from this software without prior written permission. For written
    *    permission, please contact apache@apache.org.
    *
    * 5. Products derived from this software may not be called "Apache"
    *    nor may "Apache" appear in their names without prior written
    *    permission of the Apache Group.
    *
    * THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED
    * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
    * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
    * DISCLAIMED.  IN NO EVENT SHALL THE APACHE SOFTWARE FOUNDATION OR
    * ITS CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
    * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
    * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
    * USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
    * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
    * OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
    * OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
    * SUCH DAMAGE.
    * ====================================================================
    *
    * This software consists of voluntary contributions made by many
    * individuals on behalf of the Apache Software Foundation.  For more
    * information on the Apache Software Foundation, please see
    * <http://www.apache.org/>.
    */
    using System;
    using TestProvider = org.apache.velocity.test.provider.TestProvider;

    /// <summary> Load templates from the Classpath.
    /// *
    /// </summary>
    /// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
    /// </author>
    /// <author> <a href="mailto:daveb@miceda-data.com">Dave Bryson</a>
    /// </author>
    /// <version> $Id: ClasspathResourceTest.cs,v 1.2 2003/10/27 13:54:11 corts Exp $
    ///
    /// </version>
    public class ClasspathResourceTest:BaseTestCase {
	/// <summary> VTL file extension.
	/// </summary>
	private const System.String TMPL_FILE_EXT = "vm";

	/// <summary> Comparison file extension.
	/// </summary>
	private const System.String CMP_FILE_EXT = "cmp";

	/// <summary> Comparison file extension.
	/// </summary>
	private const System.String RESULT_FILE_EXT = "res";

	/// <summary> Results relative to the build directory.
	/// </summary>
	private const System.String RESULTS_DIR = "../test/cpload/results";

	/// <summary> Results relative to the build directory.
	/// </summary>
	private const System.String COMPARE_DIR = "../test/cpload/compare";

	/// <summary> Default constructor.
	/// </summary>
	public ClasspathResourceTest():base("ClasspathResourceTest") {

	    try {
		assureResultsDirectoryExists(RESULTS_DIR);

		Velocity.setProperty(Velocity.RESOURCE_LOADER, "classpath");

		/*
		* I don't think I should have to do this, these should
		* be in the default config file.
		*/

		Velocity.addProperty("classpath." + Velocity.RESOURCE_LOADER + ".class", "org.apache.velocity.runtime.resource.loader.ClasspathResourceLoader");

		Velocity.setProperty("classpath." + Velocity.RESOURCE_LOADER + ".cache", "false");

		Velocity.setProperty("classpath." + Velocity.RESOURCE_LOADER + ".modificationCheckInterval", "2");

		Velocity.init();
	    } catch (System.Exception e) {
		System.Console.Error.WriteLine("Cannot setup ClasspathResourceTest!");
		SupportClass.WriteStackTrace(e, Console.Error);
		System.Environment.Exit(1);
	    }
	}

	/// <summary> Runs the test.
	/// </summary>
	public virtual void  runTest() {
	    try {
		/*
		*  lets ensure the results directory exists
		*/
		assureResultsDirectoryExists(RESULTS_DIR);

		Template template1 = RuntimeSingleton.getTemplate(getFileName(null, "template/test1", TMPL_FILE_EXT));

		Template template2 = RuntimeSingleton.getTemplate(getFileName(null, "template/test2", TMPL_FILE_EXT));

		System.IO.FileStream fos1 = new System.IO.FileStream(getFileName(RESULTS_DIR, "test1", RESULT_FILE_EXT), System.IO.FileMode.Create);

		System.IO.FileStream fos2 = new System.IO.FileStream(getFileName(RESULTS_DIR, "test2", RESULT_FILE_EXT), System.IO.FileMode.Create);

		//UPGRADE_ISSUE: Constructor 'java.io.BufferedWriter.BufferedWriter' was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1000_javaioBufferedWriterBufferedWriter_javaioWriter"'
		System.IO.StreamWriter writer1 = new BufferedWriter(new System.IO.StreamWriter(fos1));
		//UPGRADE_ISSUE: Constructor 'java.io.BufferedWriter.BufferedWriter' was not converted. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1000_javaioBufferedWriterBufferedWriter_javaioWriter"'
		System.IO.StreamWriter writer2 = new BufferedWriter(new System.IO.StreamWriter(fos2));

		/*
		*  put the Vector into the context, and merge both
		*/

		VelocityContext context = new VelocityContext();

		template1.merge(context, writer1);
		writer1.Flush();
		writer1.Close();

		template2.merge(context, writer2);
		writer2.Flush();
		writer2.Close();

		if (!isMatch(RESULTS_DIR, COMPARE_DIR, "test1", RESULT_FILE_EXT, CMP_FILE_EXT) || !isMatch(RESULTS_DIR, COMPARE_DIR, "test2", RESULT_FILE_EXT, CMP_FILE_EXT)) {
		    fail("Output is incorrect!");
		}
	    } catch (System.Exception e) {
		fail(e.Message);
	    }
	}
    }
}
