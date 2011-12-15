/*
 * Copyright (c) 2011 Original Authors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

/**
 * Classes in this package provide the ability to use Dependency Injection frameworks with servlets. With the help of 
 * {@link com.sharneng.webservlet.WebServlet} interface and supporting classes, one can have their servlets managed by 
 * dependency injection frameworks.
 * <p>
 * Binder for Spring Framework is provided in this package. Binder for other dependency injection framework can be
 * easily implemented by inheriting from {@link com.sharneng.webservlet.AbstractBinder} class.
 * <p>
 * Let's demonstrate the use of {@link com.sharneng.webservlet.WebServlet} by using a "Hello World" example.
 * <p>
 * First we create a servlet extends from {@link com.sharneng.webservlet.AbstractWebServlet}. It say hello to a name set
 * by {@code setName} method. The actually name will be injected through Spring.
 * <pre>
 * {@code
 * public class GreetingServlet extends AbstractWebServlet {
 *     private String name;
 * 
 *     public void setName(String name) {
 *         this.name = name;
 *     }
 * 
 *     protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
 *         resp.getOutputStream().println("Hello " + name);
 *     }
 * }
 * }
 * </pre>
 * Second, we declare a bean for our newly created servlet class in Spring context configuration and set the name
 * property to "World". This servlet now will print "Hello World".
 * <pre>
 * {@code
 * <bean id="helloWorldServlet" class="com.sharneng.webservlet.example.GreetingServlet">
 *     <property name="name" value="World" />
 * </bean>
 * }
 * </pre>
 * Lastly, let's add the servlet to the web.xml file. The servlet class must the 
 * {@link com.sharneng.webservlet.SpringBinder}. Use the servlet init parameter to connect to the real servlet.
 * <pre>
 * {@code
 * <servlet>
 *     <servlet-name>Hello World Servlet</servlet-name>
 *     <servlet-class>com.sharneng.webservlet.SpringBinder</servlet-class>
 *     <init-param>
 *         <param-name>WebServletName</param-name>
 *         <param-value>helloWorldServlet</param-value>
 *     </init-param>
 * </servlet>
 * }
 * </pre>
 * That's all to manage your servlet in dependency injection framework. Just add servlet mapping as usual and servlet
 * can be accessed.
 * 
 * @author Kenneth Xu
 */
package com.sharneng.webservlet;

