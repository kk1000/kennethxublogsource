package com.sharneng.webservlet.example;

import com.sharneng.webservlet.FilterBase;

import java.io.IOException;

import javax.servlet.FilterChain;
import javax.servlet.ServletException;
import javax.servlet.ServletRequest;
import javax.servlet.ServletResponse;

public class AppendTextFilter extends FilterBase {

    private String messageToAppend;

    public void setMessageToAppend(String messageToAppend) {
        this.messageToAppend = messageToAppend;
    }

    public void afterService(ServletRequest request, ServletResponse response) throws IOException, ServletException {
        response.getOutputStream().println("<br/>Appended by filter: " + messageToAppend);
    }
}
