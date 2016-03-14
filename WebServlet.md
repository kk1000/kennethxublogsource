# Introduction #

As servlets are typically declared in web.xml and managed by servlet container, it is difficult to inject dependencies. This project delivers the tool to let dependency injection framework managed servlet just like any other objects. Binding for Spring Framework is provided and binding to other DI framework can be easily done.

# Details #

It's best described it with an example.

## Create a servlet that requires its dependency to be injected ##
Let's use a greeting servlet. Then name is injected.

```
public class GreetingServlet extends AbstractWebServlet {
    private String name;

    public void setName(String name) {
        this.name = name;
    }

    @Override
    protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        resp.getOutputStream().println("Hello " + name);
    }

```

## Defined you servlet in your spring context ##
Declare "helloWorldServlet" bean using the servlet we just created and inject the name dependency.
```
    <bean id="helloWorldServlet" class="com.sharneng.webservlet.example.GreetingServlet">
        <property name="name" value="World"/>
    </bean>
```

## Declare the binding servlet in web.xml ##
The SpringBinder, which was available out of box for integration with Spring Frameworks, is always used. It obtains the real servlet from Spring's application context.
```
    <servlet>
        <servlet-name>Hello World Servlet</servlet-name>
        <servlet-class>com.sharneng.webservlet.SpringBinder</servlet-class>
        <init-param>
            <param-name>WebServletName</param-name>
            <param-value>helloWorldServlet</param-value>
        </init-param>
    </servlet>

    <servlet-mapping>
        <servlet-name>Hello World Servlet</servlet-name>
        <url-pattern>/hello-world/*</url-pattern>
    </servlet-mapping>
```

We are all set, you should be able to access your servlet via http://localhost:8080/web-servlet-example/hello-world/

# Get Library #
web-servlet Java library is available in Maven repository. Add below dependency to your pom and you are all set.
```
    <dependency>
        <groupId>com.sharneng</groupId>
        <artifactId>web-servlet</artifactId>
        <version>1.0.1</version>
    </dependency>
```

If you don't use Maven, you can download web-servlet-1.0.1.jar file from Maven repository:

http://search.maven.org/#artifactdetails|com.sharneng|web-servlet|1.0.1|jar

# Source Location #
This example is available in the SVN source repository:
http://kennethxublogsource.googlecode.com/svn/trunk/web-servlet