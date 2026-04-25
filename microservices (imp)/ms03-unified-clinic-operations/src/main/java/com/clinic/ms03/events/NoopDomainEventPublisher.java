package com.clinic.ms03.events;

import org.springframework.stereotype.Component;

@Component
public class NoopDomainEventPublisher implements DomainEventPublisher {

    @Override
    public void publish(String eventName, Object payload) {
        // Placeholder: replace with outbox + broker integration.
    }
}
