package com.clinic.ms03.events;

public interface DomainEventPublisher {
    void publish(String eventName, Object payload);
}
