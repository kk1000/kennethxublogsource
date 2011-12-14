package com.sharneng.webservlet.example;

import com.sharneng.webservlet.AbstractWebServlet;

import java.io.IOException;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

public class GreetingServlet extends AbstractWebServlet {
    private String name;

    public void setName(String name) {
        this.name = name;
    }

    @Override
    protected void doGet(HttpServletRequest req, HttpServletResponse resp) throws ServletException, IOException {
        resp.getOutputStream().println("Hello " + name);
    }

}
